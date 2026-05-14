namespace SnoopInSpace.Tests.Api.Auth.Payloads;

/// <summary>
/// HTTP login response payload used by integration tests.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Gets or sets JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
}
