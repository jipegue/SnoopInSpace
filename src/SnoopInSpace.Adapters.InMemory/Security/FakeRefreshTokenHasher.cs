using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Adapters.InMemory.Security;

/// <summary>
/// Fake refresh token hasher used by in-memory authentication tests.
/// </summary>
public sealed class FakeRefreshTokenHasher : IRefreshTokenHasher
{
    /// <summary>
    /// Hashes a refresh token value using a deterministic fake format.
    /// </summary>
    /// <param name="refreshToken">
    /// Refresh token raw value.
    /// </param>
    /// <returns>
    /// Fake hashed refresh token value.
    /// </returns>
    public string Hash(
        string refreshToken)
    {
        return $"FAKE_REFRESH_HASH::{refreshToken}";
    }
}
