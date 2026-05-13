using FluentAssertions;

using NSubstitute;

using SnoopInSpace.Application.Users;
using SnoopInSpace.Domain.Users;
using SnoopInSpace.Domain.Users.Exceptions;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Tests.Application.Users;

/// <summary>
/// Tests for user registration use case.
/// </summary>
public sealed class RegisterUserUseCaseTests
{
    /// <summary>
    /// Ensures registration creates a user when email is not already used.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        // Arrange
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();

        userRepository
            .GetByEmailAsync("pascal@snoop.local", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        passwordHasher
            .Hash("Password123!")
            .Returns("HASHED_PASSWORD");

        RegisterUserUseCase useCase = new(
            userRepository,
            passwordHasher);

        RegisterUserRequest request = new()
        {
            Email = "pascal@snoop.local",
            Password = "Password123!"
        };

        // Act
        RegisterUserResponse response = await useCase.ExecuteAsync(
            request,
            CancellationToken.None);

        // Assert
        response.UserId.Should().NotBeEmpty();
        response.Email.Should().Be("pascal@snoop.local");

        await userRepository
            .Received(1)
            .GetByEmailAsync("pascal@snoop.local", Arg.Any<CancellationToken>());

        passwordHasher
            .Received(1)
            .Hash("Password123!");

        await userRepository
            .Received(1)
            .CreateAsync(
                Arg.Is<User>(user =>
                    user.Email == "pascal@snoop.local"
                    && user.PasswordHash == "HASHED_PASSWORD"
                    && user.Id != Guid.Empty),
                Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Ensures registration fails when email already exists.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();

        userRepository
            .GetByEmailAsync(
                "pascal@snoop.local",
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<User?>(
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "pascal@snoop.local",
                        PasswordHash = "HASH"
                    }));

        RegisterUserUseCase useCase = new(
            userRepository,
            passwordHasher);

        RegisterUserRequest request = new()
        {
            Email = "pascal@snoop.local",
            Password = "Password123!"
        };

        // Act
        Func<Task> action = async () =>
            await useCase.ExecuteAsync(
                request,
                CancellationToken.None);

        // Assert
        await action
            .Should()
            .ThrowAsync<UserAlreadyExistsException>();
    }
}