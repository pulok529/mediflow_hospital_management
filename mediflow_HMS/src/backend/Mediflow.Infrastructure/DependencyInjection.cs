using Mediflow.Application.Abstractions.Auth;
using Mediflow.Infrastructure.Auth;
using Mediflow.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddSingleton<IIdentityService, InMemoryIdentityService>();
        return services;
    }
}
