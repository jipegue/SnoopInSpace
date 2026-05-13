namespace SnoopInSpace.Ports.Security;

/// <summary>
/// Provides password hashing features.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain password.
    /// </summary>
    /// <param name="password">Plain password.</param>
    /// <returns>Password hash.</returns>
    string Hash(string password);
}