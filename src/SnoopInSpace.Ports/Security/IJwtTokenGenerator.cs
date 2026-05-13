using SnoopInSpace.Domain.Users;

namespace SnoopInSpace.Ports.Security;

/// <summary>
/// Generates JWT access tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="user">
    /// Authenticated user.
    /// </param>
    /// <returns>
    /// JWT token string.
    /// </returns>
    string Generate(User user);
}