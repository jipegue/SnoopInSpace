using SnoopInSpace.Api.Auth;
using SnoopInSpace.Application.Security;
using SnoopInSpace.Application.Users;
using SnoopInSpace.Domain.Idempotency;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Idempotency;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SnoopInSpace.Api.Endpoints;

/// <summary>
/// Authentication endpoints.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps authentication endpoints.
    /// </summary>
    /// <param name="app">
    /// Web application.
    /// </param>
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/auth/register",
            async (
                RegisterUserApiRequest request,
                RegisterUserUseCase useCase,
                IIdempotencyStore idempotencyStore,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                string? idempotencyKey = httpContext.Request.Headers["Idempotency-Key"];

                string requestHash =
                    ComputeRegisterRequestHash(
                        request.Email,
                        request.Password);

                if (!string.IsNullOrWhiteSpace(idempotencyKey))
                {
                    IdempotencyEntry? existingEntry =
                        await idempotencyStore.GetAsync(
                            idempotencyKey,
                            cancellationToken);

                    if (existingEntry is not null)
                    {
                        if (existingEntry.RequestHash != requestHash)
                        {
                            return Results.Conflict(
                                new
                                {
                                    error = "Idempotency key was already used with a different request."
                                });
                        }

                        httpContext.Response.Headers.Location = existingEntry.Location ?? "/me";

                        return Results.Content(
                            existingEntry.ResponseBodyJson,
                            existingEntry.ContentType ?? "application/json",
                            statusCode: existingEntry.StatusCode);
                    }
                }

                try
                {
                    RegisterUserResponse response =
                        await useCase.ExecuteAsync(
                            new RegisterUserRequest
                            {
                                Email = request.Email,
                                Password = request.Password
                            },
                            cancellationToken);

                    if (!string.IsNullOrWhiteSpace(idempotencyKey))
                    {
                        string responseBodyJson =
                            JsonSerializer.Serialize(response);

                        await idempotencyStore.SaveAsync(
                            new IdempotencyEntry
                            {
                                Key = idempotencyKey,
                                RequestHash = requestHash,
                                StatusCode = StatusCodes.Status201Created,
                                Location = "/me",
                                ResponseBodyJson = responseBodyJson,
                                ContentType = "application/json",
                                CreatedAtUtc = DateTime.UtcNow
                            },
                            cancellationToken);
                    }

                    return Results.Created("/me", response);
                }
                catch (UserAlreadyExistsException exception)
                {
                    return Results.Conflict(
                        new
                        {
                            error = exception.Message
                        });
                }
            });

        app.MapPost(
            "/auth/login",
            async (
                LoginApiRequest request,
                LoginUseCase useCase,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    LoginResponse response = await useCase.ExecuteAsync(
                        new LoginRequest
                        {
                            Email = request.Email,
                            Password = request.Password
                        },
                        cancellationToken);

                    return Results.Ok(response);

                }
                catch (InvalidCredentialsException)
                {
                    return Results.Unauthorized();
                }
            });

        app.MapGet(
            "/me",
            (ClaimsPrincipal user) =>
            {
                string? userId =
                    user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

                string? email =
                    user.FindFirstValue(ClaimTypes.Email)
                    ?? user.FindFirstValue(JwtRegisteredClaimNames.Email);

                return Results.Ok(
                    new
                    {
                        UserId = userId,
                        Email = email
                    });
            })
            .RequireAuthorization();

        app.MapGet(
            "/admin",
            () => Results.Ok(
                new
                {
                    Message = "Admin access granted."
                }))
            .RequireAuthorization("AdminOnly");

        app.MapPost(
            "/auth/refresh",
            async (
                RefreshTokenRequest request,
                RefreshTokenUseCase useCase,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    RefreshTokenResponse response =
                        await useCase.ExecuteAsync(
                            request,
                            cancellationToken);

                    return Results.Ok(response);
                }
                catch (InvalidCredentialsException)
                {
                    return Results.Unauthorized();
                }
            });
    }

    /// <summary>
    /// Computes a stable hash for a register request.
    /// </summary>
    /// <param name="email">
    /// User email.
    /// </param>
    /// <param name="password">
    /// User password.
    /// </param>
    /// <returns>
    /// Stable request hash.
    /// </returns>
    private static string ComputeRegisterRequestHash(
        string email,
        string password)
    {
        string rawValue = $"{email}:{password}";

        byte[] bytes = Encoding.UTF8.GetBytes(rawValue);

        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}
