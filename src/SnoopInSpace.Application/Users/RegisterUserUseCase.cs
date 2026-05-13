using SnoopInSpace.Domain.Users;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Application.Users;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        User? existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
            throw new UserAlreadyExistsException(request.Email);

        string passwordHash = _passwordHasher.Hash(request.Password);

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _userRepository.CreateAsync(user, cancellationToken);

        return new RegisterUserResponse
        {
            UserId = user.Id,
            Email = user.Email,
        };
    }

}
