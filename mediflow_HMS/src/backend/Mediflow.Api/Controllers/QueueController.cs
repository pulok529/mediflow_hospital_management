using ApiUpdateQueueStateRequest = Mediflow.Api.Contracts.Workflow.UpdateQueueStateRequest;
using Mediflow.Application.Abstractions.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/queue")]
[Authorize(Roles = "SuperAdmin,Admin,Receptionist")]
public sealed class QueueController(IPatientWorkflowService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] DateOnly? date)
    {
        var target = date ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        return Ok(service.GetQueue(target));
    }

    [HttpPut("{appointmentId:guid}/state")]
    public IActionResult UpdateState(Guid appointmentId, [FromBody] ApiUpdateQueueStateRequest request)
    {
        var result = service.UpdateQueueState(appointmentId, new(request.State), ParseActorId(User));
        return result is null ? NotFound() : Ok(result);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
