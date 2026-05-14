using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using SnoopInSpace.Tests.Api.Auth.Payloads;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for login endpoint.
/// </summary>
public sealed class LoginEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginEndpointTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// Web application factory.
    /// </param>
    public LoginEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures login succeeds with valid credentials.
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturn200AndAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "pascal-login-success@snoop.local",
            Password = "LoginSuccess123!"
        };

        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            registerRequest);

        LoginRequest loginRequest = new()
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        LoginResponse? loginResponse =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        loginResponse!
            .AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Ensures login fails when password is invalid.
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturn401_WhenPasswordIsInvalid()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "pascal-invalid-password@snoop.local",
            Password = "InitialPassword456!"
        };

        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            registerRequest);

        LoginRequest loginRequest = new()
        {
            Email = registerRequest.Email,
            Password = "WrongPassword789!"
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Ensures login fails when email does not exist.
    /// </summary>
    [Fact]
    public async Task Login_ShouldReturn401_WhenEmailDoesNotExist()
    {
        // Arrange
        LoginRequest loginRequest = new()
        {
            Email = "pascal-unknown-user@snoop.local",
            Password = "UnknownPassword123!"
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
