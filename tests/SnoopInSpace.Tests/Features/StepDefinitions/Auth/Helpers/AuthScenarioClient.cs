using FluentAssertions;
using SnoopInSpace.Tests.Payloads.Auth;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.StepDefinitions.Auth.Helpers;

/// <summary>
/// Provides helper methods for authentication BDD scenarios.
/// </summary>
public sealed class AuthScenarioClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthScenarioClient"/> class.
    /// </summary>
    public AuthScenarioClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Sends a login request and returns the HTTP response with the parsed payload.
    /// </summary>
    /// <param name="request">
    /// Login request.
    /// </param>
    /// <returns>
    /// HTTP response and parsed login response.
    /// </returns>
    public async Task<(HttpResponseMessage Response, LoginResponse LoginResponse)> LoginAsync(
        LoginRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/auth/login",
                request,
                CancellationToken.None);

        LoginResponse? loginResponse =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        return (response, loginResponse!);
    }
}
