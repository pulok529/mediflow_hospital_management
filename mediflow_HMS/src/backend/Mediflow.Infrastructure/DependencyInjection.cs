using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.AI;
using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Application.Abstractions.Billing;
using Mediflow.Application.Abstractions.Consultation;
using Mediflow.Application.Abstractions.Diagnostics;
using Mediflow.Application.Abstractions.Nursing;
using Mediflow.Application.Abstractions.Patient;
using Mediflow.Application.Abstractions.Pharmacy;
using Mediflow.Application.Abstractions.Reporting;
using Mediflow.Infrastructure.Admission;
using Mediflow.Infrastructure.AI;
using Mediflow.Infrastructure.Audit;
using Mediflow.Infrastructure.Auth;
using Mediflow.Infrastructure.Billing;
using Mediflow.Infrastructure.Consultation;
using Mediflow.Infrastructure.Diagnostics;
using Mediflow.Infrastructure.Identity;
using Mediflow.Infrastructure.Nursing;
using Mediflow.Infrastructure.Patient;
using Mediflow.Infrastructure.Persistence;
using Mediflow.Infrastructure.Pharmacy;
using Mediflow.Infrastructure.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IAuditLogService, DatabaseAuditLogService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, DatabaseIdentityService>();
        services.AddScoped<IPatientWorkflowService, DatabasePatientWorkflowService>();
        services.AddScoped<IConsultationWorkflowService, DatabaseConsultationWorkflowService>();
        services.AddScoped<IAdmissionWorkflowService, DatabaseAdmissionWorkflowService>();
        services.AddScoped<INursingWorkflowService, InMemoryNursingWorkflowService>();
        services.AddScoped<IDiagnosticsWorkflowService, InMemoryDiagnosticsWorkflowService>();
        services.AddScoped<IPharmacyInventoryService, InMemoryPharmacyInventoryService>();
        services.AddScoped<IBillingWorkflowService, InMemoryBillingWorkflowService>();
        services.AddScoped<IReportingService, InMemoryReportingService>();
        services.AddScoped<IExternalReportBridge, ExternalReportBridge>();
        services.AddScoped<IAiWorkflowService, FastApiAiWorkflowService>();
        return services;
    }
}
