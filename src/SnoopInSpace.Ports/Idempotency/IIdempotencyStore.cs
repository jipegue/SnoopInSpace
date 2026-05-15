using SnoopInSpace.Domain.Idempotency;

namespace SnoopInSpace.Ports.Idempotency;

/// <summary>
/// Provides persistence for idempotent request entries.
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    /// Retrieves an idempotency entry by key.
    /// </summary>
    Task<IdempotencyEntry?> GetAsync(
        string key,
        CancellationToken cancellationToken);

    /// <summary>
    /// Stores a processed idempotent request entry.
    /// </summary>
    Task SaveAsync(
        IdempotencyEntry entry,
        CancellationToken cancellationToken);
}
