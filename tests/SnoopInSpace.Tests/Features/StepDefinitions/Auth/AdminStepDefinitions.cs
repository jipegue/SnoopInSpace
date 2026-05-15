using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using SnoopInSpace.Tests.Features.StepDefinitions.Auth.Helpers;
using SnoopInSpace.Tests.Payloads.Auth;

namespace SnoopInSpace.Tests.Features.StepDefinitions.Auth;

/// <summary>
/// Admin user step definitions for BDD scenarios.
/// </summary>
[Binding]
public class AdminStepDefinitions
{
    private readonly HttpClient _httpClient;
    private readonly AuthScenarioClient _authScenarioClient;
    private readonly ScenarioContext _scenarioContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminStepDefinitions"/> class.
    /// </summary>
    public AdminStepDefinitions(
        WebApplicationFactory<Program> factory,
        ScenarioContext scenarioContext)
    {
        _httpClient = factory.CreateClient();
        _authScenarioClient = new AuthScenarioClient(_httpClient);
        _scenarioContext = scenarioContext;
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

        (HttpResponseMessage response, LoginResponse loginResponse) =
            await _authScenarioClient.LoginAsync(loginRequest);

        _scenarioContext["LastLoginResponse"] = loginResponse;
        _scenarioContext["LastResponse"] = response;
    }
}
