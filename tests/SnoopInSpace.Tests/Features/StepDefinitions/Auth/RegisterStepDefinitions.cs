using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Payloads.Auth;
using System.Net.Http.Json;

namespace SnoopInSpace.Tests.Features.StepDefinitions.Auth;

/// <summary>
/// Register step definitions for BDD scenarios.
/// </summary>
[Binding]
public class RegisterStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterStepDefinitions"/> class.
    /// </summary>
    public RegisterStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Stores an idempotency key for the current scenario.
    /// </summary>
    [Given("an idempotency key {string}")]
    public void GivenAnIdempotencyKey(string idempotencyKey)
    {
        _scenarioContext["IdempotencyKey"] = idempotencyKey;
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

        _scenarioContext["CurrentRegisterRequest"] = request;

        HttpResponseMessage response = await SendRegisterRequestAsync(request);

        _scenarioContext["OriginalRegisterResponse"] = response;
        _scenarioContext["OriginalRegisterResponseBody"] =
            await response.Content.ReadAsStringAsync();

        _scenarioContext["LastResponse"] = response;
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
    /// Registers again with the same idempotency key and the same request body.
    /// </summary>
    [When("I register again with the same idempotency key")]
    public async Task WhenIRegisterAgainWithTheSameIdempotencyKey()
    {
        RegisterRequest request = _scenarioContext.Get<RegisterRequest>("CurrentRegisterRequest");

        HttpResponseMessage response = await SendRegisterRequestAsync(request);

        _scenarioContext["ReplayedRegisterResponse"] = response;
        _scenarioContext["ReplayedRegisterResponseBody"] =
            await response.Content.ReadAsStringAsync();

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Registers again with a different payload and the same idempotency key.
    /// </summary>
    [When("I register again with a different payload and the same idempotency key")]
    public async Task WhenIRegisterAgainWithADifferentPayloadAndTheSameIdempotencyKey()
    {
        RegisterRequest request = new()
        {
            Email = $"different-{Guid.NewGuid():N}@snoop.local",
            Password = "DifferentPassword123!"
        };

        HttpResponseMessage response =
            await SendRegisterRequestAsync(request);

        _scenarioContext["LastResponse"] = response;
    }

    /// <summary>
    /// Asserts that the replayed register response matches the original response.
    /// </summary>
    [Then("the replayed register response should match the original response")]
    public void ThenTheReplayedRegisterResponseShouldMatchTheOriginalResponse()
    {
        HttpResponseMessage originalResponse =
            _scenarioContext.Get<HttpResponseMessage>("OriginalRegisterResponse");

        HttpResponseMessage replayedResponse =
            _scenarioContext.Get<HttpResponseMessage>("ReplayedRegisterResponse");

        string originalResponseBody =
            _scenarioContext.Get<string>("OriginalRegisterResponseBody");

        string replayedResponseBody =
            _scenarioContext.Get<string>("ReplayedRegisterResponseBody");

        replayedResponse.StatusCode.Should().Be(originalResponse.StatusCode);
        replayedResponseBody.Should().Be(originalResponseBody);

        replayedResponse.Headers.Location.Should().Be(originalResponse.Headers.Location);
        replayedResponse.Content.Headers.ContentType?.MediaType
            .Should()
            .Be(originalResponse.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Sends a register request and applies the current idempotency key when available.
    /// </summary>
    private async Task<HttpResponseMessage> SendRegisterRequestAsync(RegisterRequest request)
    {
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, "/auth/register")
        {
            Content = JsonContent.Create(request)
        };

        if (_scenarioContext.TryGetValue("IdempotencyKey", out string? idempotencyKey))
        {
            httpRequest.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        return await _httpClient.SendAsync(httpRequest, CancellationToken.None);
    }
}
