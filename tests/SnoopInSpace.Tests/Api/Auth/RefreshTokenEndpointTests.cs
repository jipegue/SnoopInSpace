using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using SnoopInSpace.Tests.Api.Auth.Payloads;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for refresh token endpoint.
/// </summary>
public sealed class RefreshTokenEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenEndpointTests"/> class.
    /// </summary>
    public RefreshTokenEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures refresh endpoint returns a new access token when refresh token is valid.
    /// </summary>
    [Fact]
    public async Task RefreshToken_ShouldReturn200AndAccessToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "refresh-endpoint-user@snoop.local",
            Password = "RefreshPassword123!"
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
        loginResponse!.RefreshToken.Should().NotBeNullOrWhiteSpace();

        RefreshTokenRequest refreshRequest = new()
        {
            RefreshToken = loginResponse.RefreshToken
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/refresh",
                refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        RefreshTokenResponse? refreshResponse =
            await response.Content
                .ReadFromJsonAsync<RefreshTokenResponse>();

        refreshResponse.Should().NotBeNull();
        refreshResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Ensures refresh endpoint rejects invalid refresh token.
    /// </summary>
    [Fact]
    public async Task RefreshToken_ShouldReturn401_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        RefreshTokenRequest refreshRequest = new()
        {
            RefreshToken = "INVALID_REFRESH_TOKEN"
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/refresh",
                refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
