namespace SnoopInSpace.Ports.Security;

/// <summary>
/// Provides refresh token hashing operations.
/// </summary>
public interface IRefreshTokenHasher
{
    /// <summary>
    /// Hashes a refresh token value.
    /// </summary>
    /// <param name="refreshToken">
    /// Refresh token raw value.
    /// </param>
    /// <returns>
    /// Hashed refresh token value.
    /// </returns>
    string Hash(
        string refreshToken);
}
