namespace SnoopInSpace.Application.Security;

/// <summary>
/// Refresh token response.
/// </summary>
public sealed class RefreshTokenResponse
{
    /// <summary>
    /// Gets or sets new access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets new refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
