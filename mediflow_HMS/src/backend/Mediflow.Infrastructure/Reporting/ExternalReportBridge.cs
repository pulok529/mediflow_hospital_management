using Mediflow.Application.Abstractions.Reporting;
using System.Text.Json;

namespace Mediflow.Infrastructure.Reporting;

internal sealed class ExternalReportBridge : IExternalReportBridge
{
    public string BuildSsrsPayload(ReportFilter filter)
        => JsonSerializer.Serialize(new { provider = "SSRS", filter });

    public string BuildFastReportPayload(ReportFilter filter)
        => JsonSerializer.Serialize(new { provider = "FastReport", filter });
}
