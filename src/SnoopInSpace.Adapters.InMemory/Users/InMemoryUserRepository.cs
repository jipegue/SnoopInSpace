using SnoopInSpace.Domain.Users;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Adapters.InMemory.Users;

/// <summary>
/// Temporary in-memory user repository for early V1 development.
/// </summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        User? user = _users.FirstOrDefault(
            user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task CreateAsync(
        User user,
        CancellationToken cancellationToken)
    {
        _users.Add(user);

        return Task.CompletedTask;
    }
}