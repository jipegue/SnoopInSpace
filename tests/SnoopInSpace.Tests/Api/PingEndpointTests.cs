using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SnoopInSpace.Tests.Api;

/// <summary>
/// Integration tests for ping endpoint.
/// </summary>
public class PingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingEndpointTests"/> class.
    /// </summary>
    /// <param name="factory">
    /// ASP.NET test server factory.
    /// </param>
    public PingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Ensures ping endpoint returns HTTP 200.
    /// </summary>
    [Fact]
    public async Task GetPing_ShouldReturn200()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync("/ping");

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}
