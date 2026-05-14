using FluentAssertions;

using NSubstitute;

using SnoopInSpace.Application.Security;
using SnoopInSpace.Domain.Security;
using SnoopInSpace.Domain.Users;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Tests.Application.Security;

/// <summary>
/// Unit tests for <see cref="RefreshTokenUseCase"/>.
/// </summary>
public sealed class RefreshTokenUseCaseTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenUseCaseTests"/> class.
    /// </summary>
    public RefreshTokenUseCaseTests()
    {
        _refreshTokenRepository =
            Substitute.For<IRefreshTokenRepository>();

        _refreshTokenHasher =
            Substitute.For<IRefreshTokenHasher>();

        _userRepository =
            Substitute.For<IUserRepository>();

        _jwtTokenGenerator =
            Substitute.For<IJwtTokenGenerator>();
    }

    /// <summary>
    /// Ensures a new access token is returned when refresh token is valid.
    /// </summary>
    [Fact]
    public async Task RefreshToken_ShouldReturnNewAccessToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "refresh-user@snoop.local",
            PasswordHash = "HASH",
            Role = "User"
        };

        string rawRefreshToken = "RAW_REFRESH_TOKEN";
        string hashedRefreshToken = "HASHED_REFRESH_TOKEN";

        RefreshToken refreshToken = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _refreshTokenHasher
            .Hash(rawRefreshToken)
            .Returns(hashedRefreshToken);

        _refreshTokenRepository
            .GetByHashAsync(
                hashedRefreshToken,
                Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        _userRepository
            .GetByIdAsync(
                user.Id,
                Arg.Any<CancellationToken>())
            .Returns(user);

        _jwtTokenGenerator
            .Generate(user)
            .Returns("NEW_ACCESS_TOKEN");

        RefreshTokenUseCase useCase = new(
            _refreshTokenRepository,
            _refreshTokenHasher,
            _userRepository,
            _jwtTokenGenerator);

        // Act
        RefreshTokenResponse response =
            await useCase.ExecuteAsync(
                new RefreshTokenRequest
                {
                    RefreshToken = rawRefreshToken
                },
                CancellationToken.None);

        // Assert
        response.AccessToken.Should().Be("NEW_ACCESS_TOKEN");
    }
}
