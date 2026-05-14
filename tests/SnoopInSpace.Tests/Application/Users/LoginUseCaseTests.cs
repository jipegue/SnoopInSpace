using FluentAssertions;

using NSubstitute;

using SnoopInSpace.Application.Users;
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


        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "pascal@snoop.local",
            PasswordHash = "HASHED_PASSWORD",
            CreatedAtUtc = DateTime.UtcNow
        };

        userRepository
            .GetByEmailAsync("pascal@snoop.local", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        passwordVerifier
            .Verify("Password123!", "HASHED_PASSWORD")
            .Returns(true);

        jwtTokenGenerator
            .Generate(user)
            .Returns("ACCESS_TOKEN");

        LoginUseCase useCase = new(
            userRepository,
            passwordVerifier,
            jwtTokenGenerator);

        LoginRequest request = new()
        {
            Email = "pascal@snoop.local",
            Password = "Password123!"
        };

        // Act
        LoginResponse response = await useCase.ExecuteAsync(
            request,
            CancellationToken.None);

        // Assert
        response.AccessToken.Should().Be("ACCESS_TOKEN");

        await userRepository
            .Received(1)
            .GetByEmailAsync("pascal@snoop.local", Arg.Any<CancellationToken>());

        passwordVerifier
            .Received(1)
            .Verify("Password123!", "HASHED_PASSWORD");

        jwtTokenGenerator
            .Received(1)
            .Generate(user);
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

        userRepository
            .GetByEmailAsync("missing@snoop.local", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        LoginUseCase useCase = new(
            userRepository,
            passwordVerifier,
            jwtTokenGenerator);

        LoginRequest request = new()
        {
            Email = "missing@snoop.local",
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
    }
}
