using SnoopInSpace.Domain.Security;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Application.Security;

/// <summary>
/// Handles refresh token authentication flow.
/// </summary>
public sealed class RefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenUseCase"/> class.
    /// </summary>
    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenHasher refreshTokenHasher,
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenHasher = refreshTokenHasher;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Executes refresh token flow.
    /// </summary>
    public async Task<RefreshTokenResponse> ExecuteAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        string tokenHash =
            _refreshTokenHasher.Hash(request.RefreshToken);

        RefreshToken? refreshToken =
            await _refreshTokenRepository.GetByHashAsync(
                tokenHash,
                cancellationToken);

        if (refreshToken is null)
        {
            throw new InvalidCredentialsException();
        }

        if (refreshToken.RevokedAt is not null)
        {
            throw new InvalidCredentialsException();
        }

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidCredentialsException();
        }

        Domain.Users.User? user =
            await _userRepository.GetByIdAsync(
                refreshToken.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        string accessToken =
            _jwtTokenGenerator.Generate(user);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken
        };
    }
}
