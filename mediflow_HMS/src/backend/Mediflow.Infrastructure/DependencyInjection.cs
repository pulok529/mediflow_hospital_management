using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Application.Abstractions.Patient;
using Mediflow.Infrastructure.Audit;
using Mediflow.Infrastructure.Auth;
using Mediflow.Infrastructure.Identity;
using Mediflow.Infrastructure.Patient;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAuditLogService, InMemoryAuditLogService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddSingleton<IIdentityService, InMemoryIdentityService>();
        services.AddSingleton<IPatientWorkflowService, InMemoryPatientWorkflowService>();
        return services;
    }
}
