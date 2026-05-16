using ApiAssignBedRequest = Mediflow.Api.Contracts.Admission.AssignBedRequest;
using ApiCreateAdmissionRequest = Mediflow.Api.Contracts.Admission.CreateAdmissionRequest;
using ApiTransferBedRequest = Mediflow.Api.Contracts.Admission.TransferBedRequest;
using Mediflow.Application.Abstractions.Admission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/admissions")]
[Authorize(Roles = "SuperAdmin,Admin,AdmissionOfficer,FloorReceptionist")]
public sealed class AdmissionsController(IAdmissionWorkflowService service) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] ApiCreateAdmissionRequest request)
        => Ok(service.CreateAdmissionRequest(new(request.PatientId, request.PatientName, request.Source, request.DepositAmount, request.DepositReference), ParseActorId(User)));

    [HttpPut("{admissionId:guid}/assign-bed")]
    public IActionResult AssignBed(Guid admissionId, [FromBody] ApiAssignBedRequest request)
    {
        var updated = service.AssignBed(admissionId, new(request.BedId), ParseActorId(User));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{admissionId:guid}/transfer-bed")]
    public IActionResult TransferBed(Guid admissionId, [FromBody] ApiTransferBedRequest request)
    {
        var updated = service.TransferBed(admissionId, new(request.ToBedId, request.Reason), ParseActorId(User));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet]
    public IActionResult List() => Ok(service.GetAdmissions());

    [HttpGet("history/patient/{patientId:guid}")]
    public IActionResult PatientHistory(Guid patientId) => Ok(service.GetAdmissionHistory(patientId));

    [HttpGet("history/bed/{admissionId:guid}")]
    public IActionResult BedHistory(Guid admissionId) => Ok(service.GetBedHistory(admissionId));

    [HttpGet("bed-board")]
    public IActionResult BedBoard([FromQuery] Guid? floorId) => Ok(service.GetBedBoard(floorId));

    [HttpGet("floor-reception")]
    public IActionResult FloorReception([FromQuery] Guid? floorId)
    {
        var board = service.GetBedBoard(floorId);
        var admissions = service.GetAdmissions();
        return Ok(new { board, admissions });
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
