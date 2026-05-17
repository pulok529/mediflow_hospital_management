using ApiSaveConsultationRequest = Mediflow.Api.Contracts.Consultation.SaveConsultationRequest;
using ApiStartEncounterRequest = Mediflow.Api.Contracts.Consultation.StartEncounterRequest;
using Mediflow.Application.Abstractions.Consultation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/consultations")]
[Authorize(Roles = "SuperAdmin,Admin,Doctor")]
public sealed class ConsultationsController(IConsultationWorkflowService service) : ControllerBase
{
    [HttpPost("start")]
    public IActionResult Start([FromBody] ApiStartEncounterRequest request)
    {
        var result = service.StartEncounter(new(request.AppointmentId, request.PatientId, request.PatientName), ParseActorId(User));
        return Ok(result);
    }

    [HttpPut("{encounterId:guid}")]
    public IActionResult Save(Guid encounterId, [FromBody] ApiSaveConsultationRequest request)
    {
        try
        {
            var result = service.SaveConsultation(encounterId, new(request.Vitals, request.ChiefComplaint, request.Diagnosis, request.ClinicalNotes, request.Prescriptions, request.Orders, request.FollowUpDate, request.FollowUpAdvice, request.CompleteEncounter), ParseActorId(User));
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("waiting")]
    public IActionResult Waiting([FromQuery] DateOnly? date)
    {
        var target = date ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        return Ok(service.GetWaitingPatients(target));
    }

    [HttpGet("history/{patientId:guid}")]
    public IActionResult History(Guid patientId) => Ok(service.GetPatientHistory(patientId));

    [HttpGet("{encounterId:guid}")]
    public IActionResult Get(Guid encounterId)
    {
        var result = service.GetEncounter(encounterId);
        return result is null ? NotFound() : Ok(result);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
