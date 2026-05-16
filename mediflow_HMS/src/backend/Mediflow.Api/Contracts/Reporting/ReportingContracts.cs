namespace Mediflow.Api.Contracts.Reporting;

public sealed record ReportFilterRequest(DateOnly? FromDate, DateOnly? ToDate, string? Department, string? Role, string? Search);
public sealed record PrintableReportRequest(string Title, DateOnly? FromDate, DateOnly? ToDate, string? Department, string? Role, string? Search);
