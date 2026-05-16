using ApiCreateAppointmentRequest = Mediflow.Api.Contracts.Workflow.CreateAppointmentRequest;
using ApiUpdateAppointmentStateRequest = Mediflow.Api.Contracts.Workflow.UpdateAppointmentStateRequest;
using Mediflow.Application.Abstractions.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize(Roles = "SuperAdmin,Admin,Receptionist")]
public sealed class AppointmentsController(IPatientWorkflowService service) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] ApiCreateAppointmentRequest request)
    {
        var result = service.CreateAppointment(new(request.PatientId, request.AppointmentDate, request.Department, request.DoctorName), ParseActorId(User));
        return Ok(result);
    }

    [HttpGet("daily")]
    public IActionResult Daily([FromQuery] DateOnly? date)
    {
        var target = date ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        return Ok(service.GetDailyAppointments(target));
    }

    [HttpPut("{id:guid}/state")]
    public IActionResult UpdateState(Guid id, [FromBody] ApiUpdateAppointmentStateRequest request)
    {
        var result = service.UpdateAppointmentState(id, new(request.State), ParseActorId(User));
        return result is null ? NotFound() : Ok(result);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
