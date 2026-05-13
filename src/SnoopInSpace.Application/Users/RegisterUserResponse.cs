namespace SnoopInSpace.Application.Users;

/// <summary>
/// Response returned after user registration.
/// </summary>
public sealed class RegisterUserResponse
{
    /// <summary>
    /// Gets or sets the created user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the created user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}