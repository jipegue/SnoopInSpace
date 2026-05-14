using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using SnoopInSpace.Tests.Api.Auth.Payloads;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for current user endpoint.
/// </summary>
public sealed class MeEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeEndpointTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// Web application factory.
    /// </param>
    public MeEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures anonymous users cannot access /me endpoint.
    /// </summary>
    [Fact]
    public async Task Me_ShouldReturn401_WhenUserIsAnonymous()
    {
        // Act
        HttpResponseMessage response =
            await _httpClient.GetAsync("/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Ensures authenticated users can access /me endpoint.
    /// </summary>
    [Fact]
    public async Task Me_ShouldReturn200AndCurrentUser_WhenTokenIsValid()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "pascal-me@snoop.local",
            Password = "MePassword123!"
        };

        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            registerRequest);

        LoginRequest loginRequest = new()
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        HttpResponseMessage loginHttpResponse =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        LoginResponse? loginResponse =
            await loginHttpResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResponse!.AccessToken);

        // Act
        HttpResponseMessage response =
            await _httpClient.GetAsync("/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        CurrentUserResponse? currentUser =
            await response.Content
                .ReadFromJsonAsync<CurrentUserResponse>();

        currentUser.Should().NotBeNull();

        currentUser!.UserId.Should().NotBeNullOrWhiteSpace();

        currentUser.Email.Should().Be(registerRequest.Email);
    }
}
