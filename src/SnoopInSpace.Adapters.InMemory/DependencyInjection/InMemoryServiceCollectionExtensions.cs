using Microsoft.Extensions.DependencyInjection;

using SnoopInSpace.Adapters.InMemory.Security;
using SnoopInSpace.Adapters.InMemory.Users;
using SnoopInSpace.Ports.Security;
using SnoopInSpace.Ports.Users;

namespace SnoopInSpace.Adapters.InMemory.DependencyInjection;

/// <summary>
/// Provides dependency injection extensions for in-memory adapters.
/// </summary>
public static class InMemoryServiceCollectionExtensions
{
    /// <summary>
    /// Adds in-memory adapters to the service collection.
    /// </summary>
    /// <param name="services">
    /// Service collection.
    /// </param>
    /// <returns>
    /// Updated service collection.
    /// </returns>
    public static IServiceCollection AddInMemoryAdapters(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<IPasswordHasher, FakePasswordHasher>();
        services.AddScoped<IPasswordVerifier, FakePasswordVerifier>();
        services.AddScoped<IJwtTokenGenerator, FakeJwtTokenGenerator>();

        return services;
    }
}