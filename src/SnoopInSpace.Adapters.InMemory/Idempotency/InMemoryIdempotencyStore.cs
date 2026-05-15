using System.Collections.Concurrent;
using SnoopInSpace.Domain.Idempotency;
using SnoopInSpace.Ports.Idempotency;

namespace SnoopInSpace.Adapters.InMemory.Idempotency;

/// <summary>
/// In-memory implementation of <see cref="IIdempotencyStore"/>.
/// </summary>
public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, IdempotencyEntry> _entries = [];

    /// <summary>
    /// Retrieves an idempotency entry by key.
    /// </summary>
    public Task<IdempotencyEntry?> GetAsync(
        string key,
        CancellationToken cancellationToken)
    {
        _entries.TryGetValue(key, out IdempotencyEntry? entry);

        return Task.FromResult(entry);
    }

    /// <summary>
    /// Stores a processed idempotent request entry.
    /// </summary>
    public Task SaveAsync(
        IdempotencyEntry entry,
        CancellationToken cancellationToken)
    {
        _entries[entry.Key] = entry;

        return Task.CompletedTask;
    }
}
