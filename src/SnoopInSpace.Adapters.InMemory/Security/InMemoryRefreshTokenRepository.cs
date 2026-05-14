using SnoopInSpace.Domain.Security;
using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Adapters.InMemory.Security;

/// <summary>
/// Temporary In-memory refresh token repository for early V1 development.
/// </summary>
public sealed class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _refreshTokens = [];

    /// <summary>
    /// Saves a refresh token.
    /// </summary>
    public Task SaveAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        _refreshTokens.Add(refreshToken);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a refresh token by its hash.
    /// </summary>
    public Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = _refreshTokens
            .SingleOrDefault(token =>
                token.TokenHash == tokenHash);

        return Task.FromResult(refreshToken);
    }
}
