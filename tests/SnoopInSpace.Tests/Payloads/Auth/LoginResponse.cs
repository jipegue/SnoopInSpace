namespace SnoopInSpace.Tests.Payloads.Auth;

/// <summary>
/// HTTP login response payload used by integration tests.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Gets or sets JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets refresh token used to renew the access token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
