using SnoopInSpace.Application.Users;
using SnoopInSpace.Domain.Idempotency;
using SnoopInSpace.Ports.Idempotency;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SnoopInSpace.Api.Idempotency;

/// <summary>
/// Handles idempotency behavior for the register endpoint.
/// </summary>
public sealed class RegisterIdempotencyHandler
{
    private const string RegisterLocation = "/me";
    private const string JsonContentType = "application/json";

    private readonly IIdempotencyStore _idempotencyStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterIdempotencyHandler"/> class.
    /// </summary>
    public RegisterIdempotencyHandler(IIdempotencyStore idempotencyStore)
    {
        _idempotencyStore = idempotencyStore;
    }

    /// <summary>
    /// Attempts to replay a previously stored register response.
    /// </summary>
    /// <param name="httpContext">
    /// Current HTTP context.
    /// </param>
    /// <param name="idempotencyKey">
    /// Idempotency key.
    /// </param>
    /// <param name="requestHash">
    /// Stable hash of the current request.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Replayed HTTP result when a matching idempotent request exists;
    /// otherwise null.
    /// </returns>
    public async Task<IResult?> TryReplayAsync(
        HttpContext httpContext,
        string? idempotencyKey,
        string requestHash,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return null;
        }

        IdempotencyEntry? existingEntry =
            await _idempotencyStore.GetAsync(
                idempotencyKey,
                cancellationToken);

        if (existingEntry is null)
        {
            return null;
        }

        if (existingEntry.RequestHash != requestHash)
        {
            return Results.Conflict(
                new
                {
                    error = "Idempotency key was already used with a different request."
                });
        }

        httpContext.Response.Headers.Location =
            existingEntry.Location ?? RegisterLocation;

        return Results.Content(
            existingEntry.ResponseBodyJson,
            existingEntry.ContentType ?? JsonContentType,
            statusCode: existingEntry.StatusCode);
    }

    /// <summary>
    /// Saves a successful register response for future idempotent replays.
    /// </summary>
    /// <param name="idempotencyKey">
    /// Idempotency key.
    /// </param>
    /// <param name="requestHash">
    /// Stable hash of the request.
    /// </param>
    /// <param name="response">
    /// Register response to persist.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    public async Task SaveAsync(
        string? idempotencyKey,
        string requestHash,
        RegisterUserResponse response,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return;
        }

        string responseBodyJson =
            JsonSerializer.Serialize(response);

        await _idempotencyStore.SaveAsync(
            new IdempotencyEntry
            {
                Key = idempotencyKey,
                RequestHash = requestHash,
                StatusCode = StatusCodes.Status201Created,
                Location = RegisterLocation,
                ResponseBodyJson = responseBodyJson,
                ContentType = JsonContentType,
                CreatedAtUtc = DateTime.UtcNow
            },
            cancellationToken);
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
    /// Stable deterministic request hash.
    /// </returns>
    public string ComputeRequestHash(
        string email,
        string password)
    {
        string rawValue = $"{email}:{password}";

        byte[] bytes = Encoding.UTF8.GetBytes(rawValue);

        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}
