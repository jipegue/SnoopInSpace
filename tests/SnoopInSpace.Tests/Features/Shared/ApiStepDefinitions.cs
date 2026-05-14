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
    private HttpResponseMessage? _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiStepDefinitions"/> class.
    /// </summary>
    public ApiStepDefinitions(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Calls an API endpoint with GET.
    /// </summary>
    [When("I call GET {string}")]
    public async Task WhenICallGetAsync(string route)
    {
        _response = await _httpClient.GetAsync(route);
    }

    /// <summary>
    /// Asserts the latest API response status code.
    /// </summary>
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        _response.Should().NotBeNull();

        _response!.StatusCode
            .Should()
            .Be((HttpStatusCode)expectedStatusCode);
    }
}
