namespace SnoopInSpace.Application.Users;

/// <summary>
/// Response returned after successful authentication.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets refresh token used to renew the access token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
