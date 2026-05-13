namespace SnoopInSpace.Domain.Users.Exceptions;

/// <summary>
/// Thrown when a user already exists with the same email.
/// </summary>
public sealed class UserAlreadyExistsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="email">
    /// Existing user email.
    /// </param>
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.")
    {
    }
}