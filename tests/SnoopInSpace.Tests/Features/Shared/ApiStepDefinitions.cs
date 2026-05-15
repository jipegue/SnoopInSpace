using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using System.Net;

namespace SnoopInSpace.Tests.Features.Shared;

/// <summary>
/// Shared API step definitions for BDD scenarios.
/// </summary>
[Binding]
public class ApiStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiStepDefinitions"/> class.
    /// </summary>
    public ApiStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Calls an API endpoint with GET.
    /// </summary>
    [When("I call GET {string}")]
    public async Task WhenICallGetAsync(string route)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(route);
        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts the latest API response status code.
    /// </summary>
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        HttpResponseMessage response = _scenarioContext.Get<HttpResponseMessage>("LastResponse");

        response.StatusCode
            .Should()
            .Be((HttpStatusCode)expectedStatusCode);
    }
}
