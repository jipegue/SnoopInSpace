using SnoopInSpace.Domain.Users;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Application.Users;

/// <summary>
/// Handles user authentication.
/// </summary>
public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordVerifier _passwordVerifier;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUseCase"/> class.
    /// </summary>
    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordVerifier passwordVerifier,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordVerifier = passwordVerifier;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    public async Task<LoginResponse> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        bool isPasswordValid = _passwordVerifier.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException();
        }

        string accessToken = _jwtTokenGenerator.Generate(user);

        return new LoginResponse
        {
            AccessToken = accessToken
        };
    }
}