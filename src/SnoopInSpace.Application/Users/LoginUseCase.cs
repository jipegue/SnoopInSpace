using SnoopInSpace.Domain.Security;
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

    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenHasher _refreshTokenHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUseCase"/> class.
    /// </summary>
    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordVerifier passwordVerifier,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenHasher refreshTokenHasher)
    {
        _userRepository = userRepository;
        _passwordVerifier = passwordVerifier;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenHasher = refreshTokenHasher;

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

        string rawRefreshToken = Guid.NewGuid().ToString();
        string refreshTokanHash = _refreshTokenHasher.Hash(rawRefreshToken);

        RefreshToken refreshToken = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokanHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        await _refreshTokenRepository.SaveAsync(refreshToken, cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
        };
    }
}
