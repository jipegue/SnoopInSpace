namespace SnoopInSpace.Tests.Payloads.Auth;

/// <summary>
/// HTTP login request payload used by integration tests.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
