namespace SnoopInSpace.Ports.Security;

/// <summary>
/// Verifies a plain password against a stored password hash.
/// </summary>
public interface IPasswordVerifier
{
    /// <summary>
    /// Verifies whether the plain password matches the password hash.
    /// </summary>
    /// <param name="password">Plain password.</param>
    /// <param name="passwordHash">Stored password hash.</param>
    /// <returns>True when the password is valid.</returns>
    bool Verify(
        string password,
        string passwordHash);
}