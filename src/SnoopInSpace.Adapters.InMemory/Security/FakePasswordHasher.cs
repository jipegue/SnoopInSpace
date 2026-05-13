using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Adapters.InMemory.Security;

/// <summary>
/// Temporary password hasher for early V1 development.
/// </summary>
public sealed class FakePasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hash(string password)
    {
        return $"FAKE_HASH::{password}";
    }
}