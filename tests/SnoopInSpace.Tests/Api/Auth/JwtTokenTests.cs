using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SnoopInSpace.Tests.Api.Auth.Payloads;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SnoopInSpace.Tests.Api.Auth;

/// <summary>
/// Integration tests for JWT token content.
/// </summary>
public sealed class JwtTokenTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// Web application factory.
    /// </param>
    public JwtTokenTests(
        WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures JWT contains expected user claims.
    /// </summary>
    [Fact]
    public async Task Login_ShouldGenerateJwtContainingUserClaims()
    {
        // Arrange
        RegisterRequest registerRequest = new()
        {
            Email = "jwt-user@snoop.local",
            Password = "JwtPassword123!"
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
        HttpResponseMessage loginHttpResponse =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        LoginResponse? loginResponse =
            await loginHttpResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        // Assert
        loginResponse.Should().NotBeNull();

        JwtSecurityTokenHandler tokenHandler = new();

        JwtSecurityToken jwt =
            tokenHandler.ReadJwtToken(
                loginResponse!.AccessToken);

        jwt.Claims.Should().Contain(
          claim =>
              claim.Type == JwtRegisteredClaimNames.Email
              && claim.Value == registerRequest.Email);

        jwt.Claims.Should().Contain(
            claim =>
                claim.Type == JwtRegisteredClaimNames.Sub);

        jwt.Claims.Should().Contain(
           claim =>
               claim.Type == ClaimTypes.Role
               && claim.Value == "User");
    }
}
