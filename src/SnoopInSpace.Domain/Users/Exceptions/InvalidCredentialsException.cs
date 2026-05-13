namespace SnoopInSpace.Domain.Users.Exceptions;

/// <summary>
/// Thrown when authentication credentials are invalid.
/// </summary>
public sealed class InvalidCredentialsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
    /// </summary>
    public InvalidCredentialsException()
        : base("Invalid credentials.")
    {
    }
}