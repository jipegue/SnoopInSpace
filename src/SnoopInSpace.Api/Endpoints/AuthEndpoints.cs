using SnoopInSpace.Api.Auth;
using SnoopInSpace.Application.Users;
using SnoopInSpace.Domain.Users.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
                CancellationToken cancellationToken) =>
            {
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

                    return Results.Ok(response);
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

    }
}
