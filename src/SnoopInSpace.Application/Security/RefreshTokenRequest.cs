namespace SnoopInSpace.Application.Security;

/// <summary>
/// Refresh token request.
/// </summary>
public sealed class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets refresh token raw value.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
