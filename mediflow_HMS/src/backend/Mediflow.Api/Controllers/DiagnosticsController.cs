using ApiCreateImagingOrderRequest = Mediflow.Api.Contracts.Diagnostics.CreateImagingOrderRequest;
using ApiCreateImagingServiceRequest = Mediflow.Api.Contracts.Diagnostics.CreateImagingServiceRequest;
using ApiCreateLabOrderRequest = Mediflow.Api.Contracts.Diagnostics.CreateLabOrderRequest;
using ApiCreateLabTestRequest = Mediflow.Api.Contracts.Diagnostics.CreateLabTestRequest;
using ApiEnterImagingReportRequest = Mediflow.Api.Contracts.Diagnostics.EnterImagingReportRequest;
using ApiEnterLabResultRequest = Mediflow.Api.Contracts.Diagnostics.EnterLabResultRequest;
using ApiScheduleImagingRequest = Mediflow.Api.Contracts.Diagnostics.ScheduleImagingRequest;
using Mediflow.Application.Abstractions.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/diagnostics")]
[Authorize(Roles = "SuperAdmin,Admin,LabTech,Doctor")]
public sealed class DiagnosticsController(IDiagnosticsWorkflowService service) : ControllerBase
{
    [HttpPost("lab/catalog")]
    public IActionResult AddLabTest([FromBody] ApiCreateLabTestRequest request)
        => Ok(service.AddLabTest(new(request.Code, request.Name, request.Unit, request.ReferenceRange), ActorId()));

    [HttpGet("lab/catalog")]
    public IActionResult LabCatalog() => Ok(service.GetLabCatalog());

    [HttpPost("lab/orders")]
    public IActionResult CreateLabOrder([FromBody] ApiCreateLabOrderRequest request)
        => Ok(service.CreateLabOrder(new(request.AdmissionId, request.EncounterId, request.PatientName, request.TestCode), ActorId()));

    [HttpPut("lab/orders/{id:guid}/collect")]
    public IActionResult CollectSample(Guid id)
    {
        var x = service.CollectSample(id, ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpPut("lab/orders/{id:guid}/result")]
    public IActionResult EnterLabResult(Guid id, [FromBody] ApiEnterLabResultRequest request)
    {
        var x = service.EnterLabResult(id, new(request.ResultValue, request.ResultNotes, request.IsCritical), ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpPut("lab/orders/{id:guid}/approve")]
    public IActionResult ApproveLab(Guid id)
    {
        var x = service.ApproveLabResult(id, ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpGet("lab/orders")]
    public IActionResult LabOrders() => Ok(service.GetLabOrders());

    [HttpPost("imaging/catalog")]
    public IActionResult AddImagingService([FromBody] ApiCreateImagingServiceRequest request)
        => Ok(service.AddImagingService(new(request.Code, request.Name, request.Modality), ActorId()));

    [HttpGet("imaging/catalog")]
    public IActionResult ImagingCatalog() => Ok(service.GetImagingCatalog());

    [HttpPost("imaging/orders")]
    public IActionResult CreateImagingOrder([FromBody] ApiCreateImagingOrderRequest request)
        => Ok(service.CreateImagingOrder(new(request.AdmissionId, request.EncounterId, request.PatientName, request.ServiceCode), ActorId()));

    [HttpPut("imaging/orders/{id:guid}/schedule")]
    public IActionResult ScheduleImaging(Guid id, [FromBody] ApiScheduleImagingRequest request)
    {
        var x = service.ScheduleImaging(id, new(request.ScheduledAtUtc), ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpPut("imaging/orders/{id:guid}/report")]
    public IActionResult EnterImagingReport(Guid id, [FromBody] ApiEnterImagingReportRequest request)
    {
        var x = service.EnterImagingReport(id, new(request.ReportText, request.FileReference), ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpPut("imaging/orders/{id:guid}/approve")]
    public IActionResult ApproveImaging(Guid id)
    {
        var x = service.ApproveImagingReport(id, ActorId());
        return x is null ? NotFound() : Ok(x);
    }

    [HttpGet("imaging/orders")]
    public IActionResult ImagingOrders() => Ok(service.GetImagingOrders());

    private Guid? ActorId() => Guid.TryParse(User.FindFirstValue("sub"), out var id) ? id : null;
}
