namespace SnoopInSpace.Application.Users;

/// <summary>
/// Request used to register a new user.
/// </summary>
public sealed class RegisterUserRequest
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