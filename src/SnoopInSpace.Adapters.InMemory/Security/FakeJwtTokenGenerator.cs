using SnoopInSpace.Domain.Users;
using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Adapters.InMemory.Security;

/// <summary>
/// Temporary JWT token generator for early V1 development.
/// </summary>
public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    /// <inheritdoc />
    public string Generate(User user)
    {
        return $"FAKE_JWT::{user.Id}";
    }
}