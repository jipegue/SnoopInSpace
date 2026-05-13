using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for register endpoint.
/// </summary>
public sealed class RegisterEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterEndpointTests"/> class.
    /// </summary>
    public RegisterEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures registration succeeds with a new email.
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturn200_WhenEmailIsAvailable()
    {
        // Arrange
        RegisterRequest request = new()
        {
            Email = "pascal@snoop.local",
            Password = "Password123!"
        };

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/register",
                request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Ensures registration fails when email already exists.
    /// </summary>
    [Fact]
    public async Task Register_ShouldReturn409_WhenEmailAlreadyExists()
    {
        // Arrange
        RegisterRequest request = new()
        {
            Email = "duplicate@snoop.local",
            Password = "Password123!"
        };

        // First registration
        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            request);

        // Act
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/register",
                request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// HTTP request payload.
    /// </summary>
    private sealed class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}