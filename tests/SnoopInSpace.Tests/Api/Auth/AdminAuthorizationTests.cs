using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using SnoopInSpace.Tests.Api.Auth.Payloads;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for admin authorization.
/// </summary>
public sealed class AdminAuthorizationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthorizationTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// Web application factory.
    /// </param>
    public AdminAuthorizationTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures anonymous users cannot access admin endpoint.
    /// </summary>
    [Fact]
    public async Task AdminEndpoint_ShouldReturn401_WhenUserIsAnonymous()
    {
        // Act
        HttpResponseMessage response =
            await _httpClient.GetAsync("/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Ensures authenticated non-admin users cannot access admin endpoint.
    /// </summary>
    [Fact]
    public async Task AdminEndpoint_ShouldReturn403_WhenUserIsNotAdmin()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "standard-user@snoop.local",
            Password = "StandardPassword123!"
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
            await _httpClient.GetAsync("/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Ensures authenticated admin users can access admin endpoint.
    /// </summary>
    [Fact]
    public async Task AdminEndpoint_ShouldReturn200_WhenUserIsAdmin()
    {
        // Arrange
        LoginRequest loginRequest = new()
        {
            Email = "admin@snoop.local",
            Password = "Admin123!"
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
            await _httpClient.GetAsync("/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
