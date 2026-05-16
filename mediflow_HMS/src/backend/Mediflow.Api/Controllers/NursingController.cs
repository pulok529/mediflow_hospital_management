using ApiAddIntakeOutputRequest = Mediflow.Api.Contracts.Nursing.AddIntakeOutputRequest;
using ApiAddMedicationRequest = Mediflow.Api.Contracts.Nursing.AddMedicationRequest;
using ApiAddShiftHandoverRequest = Mediflow.Api.Contracts.Nursing.AddShiftHandoverRequest;
using ApiAdministerMedicationRequest = Mediflow.Api.Contracts.Nursing.AdministerMedicationRequest;
using ApiCreateNursingNoteRequest = Mediflow.Api.Contracts.Nursing.CreateNursingNoteRequest;
using ApiRecordVitalsRequest = Mediflow.Api.Contracts.Nursing.RecordVitalsRequest;
using ApiScheduleVitalsRequest = Mediflow.Api.Contracts.Nursing.ScheduleVitalsRequest;
using Mediflow.Application.Abstractions.Nursing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/nursing")]
[Authorize(Roles = "SuperAdmin,Admin,Nurse,FloorReceptionist")]
public sealed class NursingController(INursingWorkflowService service) : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult Dashboard() => Ok(new
    {
        assignedPatients = service.GetAssignedPatients(),
        medicationDue = service.GetMedicationDueList(DateTime.UtcNow),
        alerts = service.GetActiveAlerts(),
        transferDischarge = service.GetTransferDischargePreparation()
    });

    [HttpPost("notes")]
    public IActionResult AddNote([FromBody] ApiCreateNursingNoteRequest request)
        => Ok(service.AddNursingNote(new(request.AdmissionId, request.NurseName, request.Note, request.LinkedDoctorRoundEncounterId), ParseActorId(User)));

    [HttpGet("notes/{admissionId:guid}")]
    public IActionResult GetNotes(Guid admissionId) => Ok(service.GetNursingNotes(admissionId));

    [HttpPost("medications")]
    public IActionResult AddMedication([FromBody] ApiAddMedicationRequest request)
        => Ok(service.AddMedication(new(request.AdmissionId, request.MedicationName, request.Dose, request.DueAtUtc), ParseActorId(User)));

    [HttpPut("medications/{medicationId:guid}/administer")]
    public IActionResult Administer(Guid medicationId, [FromBody] ApiAdministerMedicationRequest request)
    {
        var updated = service.AdministerMedication(medicationId, new(request.Remarks), ParseActorId(User));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("medications/due")]
    public IActionResult DueMedications() => Ok(service.GetMedicationDueList(DateTime.UtcNow));

    [HttpPost("vitals/schedule")]
    public IActionResult ScheduleVitals([FromBody] ApiScheduleVitalsRequest request)
        => Ok(service.ScheduleVitals(new(request.AdmissionId, request.DueAtUtc), ParseActorId(User)));

    [HttpPut("vitals/{vitalsScheduleId:guid}/record")]
    public IActionResult RecordVitals(Guid vitalsScheduleId, [FromBody] ApiRecordVitalsRequest request)
    {
        var recorded = service.RecordVitals(vitalsScheduleId, new(request.TemperatureC, request.Pulse, request.SystolicBp, request.DiastolicBp, request.SpO2), ParseActorId(User));
        return recorded is null ? NotFound() : Ok(recorded);
    }

    [HttpGet("vitals/{admissionId:guid}")]
    public IActionResult Vitals(Guid admissionId) => Ok(service.GetVitalsSchedules(admissionId));

    [HttpPost("io")]
    public IActionResult AddIo([FromBody] ApiAddIntakeOutputRequest request)
        => Ok(service.AddIntakeOutput(new(request.AdmissionId, request.IntakeMl, request.OutputMl, request.Notes), ParseActorId(User)));

    [HttpGet("io/{admissionId:guid}")]
    public IActionResult GetIo(Guid admissionId) => Ok(service.GetIntakeOutput(admissionId));

    [HttpPost("handover")]
    public IActionResult AddHandover([FromBody] ApiAddShiftHandoverRequest request)
        => Ok(service.AddShiftHandover(new(request.AdmissionId, request.FromShift, request.ToShift, request.Summary), ParseActorId(User)));

    [HttpGet("handover/{admissionId:guid}")]
    public IActionResult Handover(Guid admissionId) => Ok(service.GetShiftHandovers(admissionId));

    [HttpGet("alerts")]
    public IActionResult Alerts() => Ok(service.GetActiveAlerts());

    [HttpGet("transfer-discharge")]
    public IActionResult TransferDischarge() => Ok(service.GetTransferDischargePreparation());

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
