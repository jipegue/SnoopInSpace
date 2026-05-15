namespace SnoopInSpace.Tests.Payloads.Auth;

/// <summary>
/// HTTP current user response payload used by integration tests.
/// </summary>
public sealed class CurrentUserResponse
{
    /// <summary>
    /// Gets or sets authenticated user identifier.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets authenticated user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
