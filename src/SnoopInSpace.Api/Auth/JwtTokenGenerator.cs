using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using SnoopInSpace.Domain.Users;
using SnoopInSpace.Ports.Security;

namespace SnoopInSpace.Api.Auth;

/// <summary>
/// Generates JWT access tokens.
/// </summary>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenGenerator"/> class.
    /// </summary>
    public JwtTokenGenerator(
        JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /// <inheritdoc />
    public string Generate(User user)
    {
        byte[] signingKeyBytes =
            Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(signingKeyBytes),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
        ];

        DateTime expiresAtUtc =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes);

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);
    }
}
