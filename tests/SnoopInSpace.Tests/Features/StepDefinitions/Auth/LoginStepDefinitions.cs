using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Payloads.Auth;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.StepDefinitions.Auth;

/// <summary>
/// Login step definitions for BDD scenarios.
/// </summary>
[Binding]
public class LoginStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginStepDefinitions"/> class.
    /// </summary>
    public LoginStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Logs in with the current registered user's credentials.
    /// </summary>
    [When("I login with valid credentials")]
    public async Task WhenILoginWithValidCredentials()
    {
        RegisterRequest request = _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        LoginRequest loginRequest = new()
        {
            Email = request.Email,
            Password = request.Password,
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/login",
            loginRequest, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;

        LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        _scenarioContext["LastLoginResponse"] = loginResponse!;
    }

    /// <summary>
    /// Logs in with the current registered user's email and an invalid password.
    /// </summary>
    [When("I login with an invalid password")]
    public async Task WhenILoginWithAnInvalidPassword()
    {
        RegisterRequest request = _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        LoginRequest loginRequest = new()
        {
            Email = request.Email,
            Password = "wrongPassword",
        };

        HttpResponseMessage loginResponse = await _httpClient.PostAsJsonAsync(
            "/auth/login",
            loginRequest, CancellationToken.None);

        loginResponse.Should().NotBeNull();

        _scenarioContext["LastResponse"] = loginResponse;
    }

    /// <summary>
    /// Attempts login with an unknown email.
    /// </summary>
    [When("I login with an unknown email")]
    public async Task WhenILoginWithAnUnknownEmail()
    {
        LoginRequest loginRequest = new()
        {
            Email = $"unknown-{Guid.NewGuid():N}@snoop.local",
            Password = "unknownPassword",
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/login",
            loginRequest, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts that a JWT access token was returned.
    /// </summary>
    [Then("a JWT token should be returned")]
    public void ThenAJWTTokenShouldBeReturned()
    {
        LoginResponse loginResponse = _scenarioContext.Get<LoginResponse>("LastLoginResponse");

        loginResponse.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Asserts that a refresh token was returned.
    /// </summary>
    [Then("a refresh token should be returned")]
    public void ThenARefreshTokenShouldBeReturned()
    {
        LoginResponse loginResponse = _scenarioContext.Get<LoginResponse>("LastLoginResponse");

        loginResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}
