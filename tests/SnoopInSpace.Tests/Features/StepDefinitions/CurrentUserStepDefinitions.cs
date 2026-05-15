using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Payloads.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.StepDefinitions;

/// <summary>
/// Current user step definitions for BDD scenarios.
/// </summary>
[Binding]
public class CurrentUserStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserStepDefinitions"/> class.
    /// </summary>
    public CurrentUserStepDefinitions(
        WebApplicationFactory<Program> factory,
        ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Calls an API endpoint with GET using the latest JWT token.
    /// </summary>
    [When("I call GET {string} with the JWT token")]
    public async Task WhenICallGETWithTheJWTToken(string route)
    {
        LoginResponse loginResponse =
            _scenarioContext.Get<LoginResponse>("LastLoginResponse");

        using HttpRequestMessage request = new(
            HttpMethod.Get,
            route);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginResponse.AccessToken);

        HttpResponseMessage response =
            await _httpClient.SendAsync(request);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts that the current user email matches the registered user email.
    /// </summary>
    [Then("the current user email should match the registered user email")]
    public async Task ThenTheCurrentUserEmailShouldMatchTheRegisteredUserEmail()
    {
        RegisterRequest registerRequest =
            _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        HttpResponseMessage response =
            _scenarioContext.Get<HttpResponseMessage>("LastResponse");

        CurrentUserResponse? currentUser =
            await response.Content.ReadFromJsonAsync<CurrentUserResponse>();

        currentUser.Should().NotBeNull();

        currentUser!.Email.Should().Be(registerRequest.Email);
    }
}
