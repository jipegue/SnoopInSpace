using SnoopInSpace.Domain.Security;

namespace SnoopInSpace.Ports.Security;

/// <summary>
/// Provides refresh token persistence operations.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Saves a refresh token.
    /// </summary>
    /// <param name="refreshToken">
    /// Refresh token to save.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    Task SaveAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a refresh token by its hash.
    /// </summary>
    /// <param name="tokenHash">
    /// Refresh token hash.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);
}
