using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SnoopInSpace.Api.Auth;
using SnoopInSpace.Ports.Security;
using System.Text;

namespace SnoopInSpace.Api.DependencyInjection;

/// <summary>
/// Provides JWT authentication dependency injection extensions.
/// </summary>
public static class JwtAuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Adds JWT authentication services.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSettings jwtSettings =
            configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are missing.");

        services.AddSingleton(jwtSettings);

        byte[] signingKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}