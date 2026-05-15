namespace SnoopInSpace.Tests.Payloads.Auth
{
    /// <summary>
    /// HTTP request payload.
    /// </summary>
    public sealed class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
