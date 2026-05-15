namespace SnoopInSpace.Domain.Security;

/// <summary>
/// Represents a refresh token used to renew an access token.
/// </summary>
public sealed class RefreshToken
{
    /// <summary>
    /// Gets or sets refresh token identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets user identifier linked to the refresh token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets hashed refresh token value.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets refresh token expiration date.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets refresh token revocation date.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets refresh token creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
