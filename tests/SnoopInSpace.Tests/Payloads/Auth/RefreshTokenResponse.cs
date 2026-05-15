namespace SnoopInSpace.Tests.Payloads.Auth;

/// <summary>
/// HTTP refresh token response payload used by integration tests.
/// </summary>
public sealed class RefreshTokenResponse
{
    /// <summary>
    /// Gets or sets new JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets rotated refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
