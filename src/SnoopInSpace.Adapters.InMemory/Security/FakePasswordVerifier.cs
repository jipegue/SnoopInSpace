using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Adapters.InMemory.Security;

/// <summary>
/// Temporary password verifier for early V1 development.
/// </summary>
public sealed class FakePasswordVerifier : IPasswordVerifier
{
    /// <inheritdoc />
    public bool Verify(
        string password,
        string passwordHash)
    {
        return passwordHash == $"FAKE_HASH::{password}";
    }
}