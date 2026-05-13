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
}