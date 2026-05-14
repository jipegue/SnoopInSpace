using SnoopInSpace.Domain.Users;

namespace SnoopInSpace.Ports.Users;

/// <summary>
/// Repository abstraction for users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by email.
    /// </summary>
    /// <param name="email">
    /// User email.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Matching user or null.
    /// </returns>
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="user">
    /// User to create.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    Task CreateAsync(
        User user,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by identifier.
    /// </summary>
    /// <param name="id">
    /// User identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Matching user or null.
    /// </returns>
    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}
