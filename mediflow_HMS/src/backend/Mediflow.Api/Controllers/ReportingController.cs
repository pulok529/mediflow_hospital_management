using Mediflow.Api.Contracts.Reporting;
using Mediflow.Application.Abstractions.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/reporting")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class ReportingController(IReportingService reporting, IExternalReportBridge externalBridge) : ControllerBase
{
    [HttpGet("dashboard/{role}")]
    public IActionResult RoleDashboard(string role, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, [FromQuery] string? department, [FromQuery] string? search)
        => Ok(reporting.GetDashboard(role, new ReportFilter(fromDate, toDate, department, role, search)));

    [HttpPost("datasets")]
    public IActionResult Datasets([FromBody] ReportFilterRequest request)
        => Ok(reporting.GetReportDatasets(new ReportFilter(request.FromDate, request.ToDate, request.Department, request.Role, request.Search)));

    [HttpPost("printable")]
    public IActionResult Printable([FromBody] PrintableReportRequest request)
    {
        var bytes = reporting.GeneratePrintableSummary(request.Title, new ReportFilter(request.FromDate, request.ToDate, request.Department, request.Role, request.Search));
        return File(bytes, "application/pdf", "report.pdf");
    }

    [HttpPost("ssrs-payload")]
    public IActionResult SsrsPayload([FromBody] ReportFilterRequest request)
        => Ok(externalBridge.BuildSsrsPayload(new ReportFilter(request.FromDate, request.ToDate, request.Department, request.Role, request.Search)));

    [HttpPost("fastreport-payload")]
    public IActionResult FastReportPayload([FromBody] ReportFilterRequest request)
        => Ok(externalBridge.BuildFastReportPayload(new ReportFilter(request.FromDate, request.ToDate, request.Department, request.Role, request.Search)));

    [HttpGet("audit-logs")]
    public IActionResult AuditLogs([FromServices] Mediflow.Application.Abstractions.Audit.IAuditLogService audit)
        => Ok(audit.GetLatest(500));
}
