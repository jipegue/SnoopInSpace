using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Payloads.Auth;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.StepDefinitions.Auth;

/// <summary>
/// Refresh token step definitions for BDD scenarios.
/// </summary>
[Binding]
public class RefreshTokenStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenStepDefinitions"/> class.
    /// </summary>
    public RefreshTokenStepDefinitions(
        WebApplicationFactory<Program> factory,
        ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Refreshes the access token using the latest refresh token.
    /// </summary>
    [When("I refresh the access token")]
    public async Task WhenIRefreshTheAccessToken()
    {
        LoginResponse loginResponse = _scenarioContext.Get<LoginResponse>("LastLoginResponse");

        _scenarioContext["PreviousRefreshToken"] = loginResponse.RefreshToken;

        RefreshTokenRequest request = new()
        {
            RefreshToken = loginResponse.RefreshToken,
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/refresh",
            request, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;

        RefreshTokenResponse? refreshResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        refreshResponse.Should().NotBeNull();

        _scenarioContext["LastRefreshTokenResponse"] = refreshResponse;
    }

    /// <summary>
    /// Attempts to reuse the previous refresh token after rotation.
    /// </summary>
    [When("I try to reuse the previous refresh token")]
    public async Task WhenITryToReuseThePreviousRefreshToken()
    {
        string previousRefreshToken = _scenarioContext.Get<string>("PreviousRefreshToken");

        RefreshTokenRequest request = new()
        {
            RefreshToken = previousRefreshToken,
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/refresh",
            request, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Attempts to refresh the access token with an invalid refresh token.
    /// </summary>
    [When("I refresh the access token with an invalid refresh token")]
    public async Task WhenIRefreshTheAccessTokenWithAnInvalidRefreshToken()
    {
        RefreshTokenRequest request = new()
        {
            RefreshToken = "INVALID_REFRESH_TOKEN",
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/refresh",
            request,
            CancellationToken.None);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts that a new JWT access token was returned.
    /// </summary>
    [Then("a new JWT token should be returned")]
    public void ThenANewJWTTokenShouldBeReturned()
    {
        RefreshTokenResponse refreshResponse = _scenarioContext.Get<RefreshTokenResponse>("LastRefreshTokenResponse");

        refreshResponse.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Asserts that a new refresh token was returned.
    /// </summary>
    [Then("a new refresh token should be returned")]
    public void ThenANewRefreshTokenShouldBeReturned()
    {
        string previousRefreshToken = _scenarioContext.Get<string>("PreviousRefreshToken");

        RefreshTokenResponse refreshResponse = _scenarioContext.Get<RefreshTokenResponse>("LastRefreshTokenResponse");

        refreshResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        refreshResponse.RefreshToken.Should().NotBe(previousRefreshToken);
    }
}
