using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Api.Auth.Payloads;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.Shared;

/// <summary>
/// Authentication step definitions for BDD scenarios.
/// </summary>
[Binding]
public class AuthStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthStepDefinitions"/> class.
    /// </summary>
    public AuthStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Registers a user with valid credentials.
    /// </summary>
    [When("I register with valid credentials")]
    public async Task WhenIRegisterWithValidCredentials()
    {
        RegisterRequest request = new()
        {
            Email = $"register-{Guid.NewGuid():N}@snoop.local",
            Password = "Password123!"
        };

        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/register",
                request, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Creates an existing registered user for the current scenario.
    /// </summary>
    [Given("a registered user already exists")]
    public async Task GivenARegisteredUserAlreadyExists()
    {
        RegisterRequest request = new()
        {
            Email = $"existing-{Guid.NewGuid():N}@snoop.local",
            Password = "Password123!"
        };

        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            request, CancellationToken.None);

        _scenarioContext["CurrentRegisterRequest"] = request;
    }

    /// <summary>
    /// Registers again with the same email as the existing user.
    /// </summary>
    [When("I register with the same email")]
    public async Task WhenIRegisterWithTheSameEmail()
    {
        RegisterRequest request = _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/auth/register",
            request, CancellationToken.None);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Creates a registered user for the current scenario.
    /// </summary>
    [Given("a registered user exists")]
    public async Task GivenARegisteredUserExists()
    {
        RegisterRequest request = new()
        {
            Email = $"login-{Guid.NewGuid():N}@snoop.local",
            Password = "Password123!"
        };

        await _httpClient.PostAsJsonAsync(
            "/auth/register",
            request, CancellationToken.None);

        _scenarioContext["CurrentRegisterRequest"] = request;
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

    /// <summary>
    /// Calls an API endpoint with GET using the latest JWT token.
    /// </summary>
    [When("I call GET {string} with the JWT token")]
    public async Task WhenICallGETWithTheJWTToken(string route)
    {
        LoginResponse loginResponse = _scenarioContext.Get<LoginResponse>("LastLoginResponse");

        using HttpRequestMessage request = new(
            HttpMethod.Get,
            route);

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            loginResponse.AccessToken);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts that the current user email matches the registered user email.
    /// </summary>
    [Then("the current user email should match the registered user email")]
    public async Task ThenTheCurrentUserEmailShouldMatchTheRegisteredUserEmail()
    {
        RegisterRequest registerRequest = _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        HttpResponseMessage response = _scenarioContext.Get<HttpResponseMessage>("LastResponse");

        CurrentUserResponse? currentUser = await response.Content.ReadFromJsonAsync<CurrentUserResponse>();

        currentUser.Should().NotBeNull();
        currentUser!.Email.Should().Be(registerRequest.Email);
    }

    /// <summary>
    /// Stores the seeded admin credentials for the current scenario.
    /// </summary>
    [Given("the seeded admin user exists")]
    public void GivenTheSeededAdminUserExists()
    {
        RegisterRequest request = new()
        {
            Email = "admin@snoop.local",
            Password = "Admin123!",
        };

        _scenarioContext["CurrentRegisterRequest"] = request;
    }

    /// <summary>
    /// Logs in as the seeded admin user.
    /// </summary>
    [When("I login as seeded admin")]
    public async Task WhenILoginAsSeededAdmin()
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

        LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        _scenarioContext["LastLoginResponse"] = loginResponse;
        _scenarioContext["LastResponse"] = response;
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
}
