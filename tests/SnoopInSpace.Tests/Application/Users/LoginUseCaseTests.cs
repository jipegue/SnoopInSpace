using FluentAssertions;

using NSubstitute;

using SnoopInSpace.Application.Users;
using SnoopInSpace.Domain.Security;
using SnoopInSpace.Domain.Users;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Tests.Application.Users;

/// <summary>
/// Tests for login use case.
/// </summary>
public sealed class LoginUseCaseTests
{
    /// <summary>
    /// Ensures login returns an access token when credentials are valid.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldReturnAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IPasswordVerifier passwordVerifier = Substitute.For<IPasswordVerifier>();
        IJwtTokenGenerator jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

        IRefreshTokenHasher refreshTokenHasher = Substitute.For<IRefreshTokenHasher>();
        IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "login-user@snoop.local",
            PasswordHash = "HASHED_PASSWORD",
            CreatedAtUtc = DateTime.UtcNow
        };

        userRepository
            .GetByEmailAsync("login-user@snoop.local", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        passwordVerifier
            .Verify("Password123!", "HASHED_PASSWORD")
            .Returns(true);

        jwtTokenGenerator
            .Generate(user)
            .Returns("ACCESS_TOKEN");

        refreshTokenHasher.Hash(Arg.Any<string>())
            .Returns("HASHED_REFRESH_TOKEN");

        LoginUseCase useCase = new(
            userRepository,
            passwordVerifier,
            jwtTokenGenerator,
            refreshTokenRepository,
            refreshTokenHasher);

        LoginRequest request = new()
        {
            Email = "login-user@snoop.local",
            Password = "Password123!"
        };

        // Act
        LoginResponse response = await useCase.ExecuteAsync(
            request,
            CancellationToken.None);

        // Assert
        response.AccessToken.Should().Be("ACCESS_TOKEN");
        response.RefreshToken.Should().NotBeNullOrWhiteSpace();

        await userRepository
            .Received(1)
            .GetByEmailAsync("login-user@snoop.local", Arg.Any<CancellationToken>());

        passwordVerifier
            .Received(1)
            .Verify("Password123!", "HASHED_PASSWORD");

        jwtTokenGenerator
            .Received(1)
            .Generate(user);

        await refreshTokenRepository
            .Received(1)
            .SaveAsync(
                Arg.Is<RefreshToken>(
                    token =>
                    token.UserId == user.Id
                    && token.TokenHash == "HASHED_REFRESH_TOKEN"
                    && token.RevokedAt == null
                    && token.ExpiresAt > DateTime.UtcNow),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Ensures login fails when user does not exist.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IPasswordVerifier passwordVerifier = Substitute.For<IPasswordVerifier>();
        IJwtTokenGenerator jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

        IRefreshTokenHasher refreshTokenHasher = Substitute.For<IRefreshTokenHasher>();
        IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();


        userRepository
            .GetByEmailAsync("missing-user@snoop.local", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        LoginUseCase useCase = new(
            userRepository,
            passwordVerifier,
            jwtTokenGenerator,
            refreshTokenRepository,
            refreshTokenHasher);

        LoginRequest request = new()
        {
            Email = "missing-user@snoop.local",
            Password = "Password123!"
        };

        // Act
        Func<Task> action = async () =>
            await useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await action
            .Should()
            .ThrowAsync<InvalidCredentialsException>();

        passwordVerifier
            .DidNotReceive()
            .Verify(Arg.Any<string>(), Arg.Any<string>());

        jwtTokenGenerator
            .DidNotReceive()
            .Generate(Arg.Any<User>());

        await refreshTokenRepository
            .DidNotReceive()
            .SaveAsync(
                Arg.Any<RefreshToken>(),
                Arg.Any<CancellationToken>());

        refreshTokenHasher
            .DidNotReceive()
            .Hash(Arg.Any<string>());
    }
}
