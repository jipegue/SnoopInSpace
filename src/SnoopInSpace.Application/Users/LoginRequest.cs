namespace SnoopInSpace.Application.Users;

/// <summary>
/// Request used to authenticate a user.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plain password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}