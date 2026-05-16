using Mediflow.Application.Abstractions.Admission;
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
using Mediflow.Infrastructure.Audit;
using Mediflow.Infrastructure.Auth;
using Mediflow.Infrastructure.Billing;
using Mediflow.Infrastructure.Consultation;
using Mediflow.Infrastructure.Diagnostics;
using Mediflow.Infrastructure.Identity;
using Mediflow.Infrastructure.Nursing;
using Mediflow.Infrastructure.Patient;
using Mediflow.Infrastructure.Pharmacy;
using Mediflow.Infrastructure.Reporting;
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
        services.AddSingleton<IConsultationWorkflowService, InMemoryConsultationWorkflowService>();
        services.AddSingleton<IAdmissionWorkflowService, InMemoryAdmissionWorkflowService>();
        services.AddSingleton<INursingWorkflowService, InMemoryNursingWorkflowService>();
        services.AddSingleton<IDiagnosticsWorkflowService, InMemoryDiagnosticsWorkflowService>();
        services.AddSingleton<IPharmacyInventoryService, InMemoryPharmacyInventoryService>();
        services.AddSingleton<IBillingWorkflowService, InMemoryBillingWorkflowService>();
        services.AddSingleton<IReportingService, InMemoryReportingService>();
        services.AddSingleton<IExternalReportBridge, ExternalReportBridge>();
        return services;
    }
}
