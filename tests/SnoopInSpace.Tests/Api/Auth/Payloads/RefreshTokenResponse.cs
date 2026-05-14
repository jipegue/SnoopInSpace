namespace SnoopInSpace.Tests.Api.Auth.Payloads;

/// <summary>
/// HTTP refresh token response payload used by integration tests.
/// </summary>
public sealed class RefreshTokenResponse
{
    /// <summary>
    /// Gets or sets new JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
}
