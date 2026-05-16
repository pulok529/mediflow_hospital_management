namespace Mediflow.Application.Abstractions.Reporting;

public sealed record KpiCard(string Key, string Label, decimal Value, string Unit);
public sealed record DashboardData(string Role, IReadOnlyCollection<KpiCard> Kpis);
public sealed record ReportFilter(DateOnly? FromDate, DateOnly? ToDate, string? Department, string? Role, string? Search);
public sealed record ExportDataset(string Name, IReadOnlyCollection<IDictionary<string, object?>> Rows);

public interface IReportingService
{
    DashboardData GetDashboard(string role, ReportFilter filter);
    IReadOnlyCollection<ExportDataset> GetReportDatasets(ReportFilter filter);
    byte[] GeneratePrintableSummary(string title, ReportFilter filter);
}

public interface IExternalReportBridge
{
    string BuildSsrsPayload(ReportFilter filter);
    string BuildFastReportPayload(ReportFilter filter);
}
