namespace SnoopInSpace.Domain.Idempotency;

/// <summary>
/// Represents a processed idempotent request entry.
/// </summary>
public sealed class IdempotencyEntry
{
    /// <summary>
    /// Gets or sets the idempotency key.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Gets or sets the request hash.
    /// </summary>
    public required string RequestHash { get; init; }

    /// <summary>
    /// Gets or sets the HTTP status code returned for the request.
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// Gets or sets the creation date in UTC.
    /// </summary>
    public required DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets or sets the serialized response body.
    /// </summary>
    public string? ResponseBodyJson { get; init; }

    /// <summary>
    /// Gets or sets the response content type.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets or sets the response location.
    /// </summary>
    public string? Location { get; init; }
}
