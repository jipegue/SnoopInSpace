namespace SnoopInSpace.Tests.Api.Auth.Payloads;

/// <summary>
/// HTTP refresh token request payload used by integration tests.
/// </summary>
public sealed class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets refresh token value.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
