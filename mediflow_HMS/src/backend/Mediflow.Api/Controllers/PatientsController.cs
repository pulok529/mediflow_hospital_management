using ApiCreatePatientRequest = Mediflow.Api.Contracts.Workflow.CreatePatientRequest;
using ApiUpdatePatientRequest = Mediflow.Api.Contracts.Workflow.UpdatePatientRequest;
using Mediflow.Application.Abstractions.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize(Roles = "SuperAdmin,Admin,Receptionist")]
public sealed class PatientsController(IPatientWorkflowService service) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] ApiCreatePatientRequest request)
    {
        var result = service.CreatePatient(new(request.FullName, request.DateOfBirth, request.Phone, request.Gender, request.Address), ParseActorId(User));
        return Ok(result);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? patientCode, [FromQuery] string? name, [FromQuery] string? phone, [FromQuery] DateOnly? dateOfBirth)
    {
        var result = service.SearchPatients(new(patientCode, name, phone, dateOfBirth));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] ApiUpdatePatientRequest request)
    {
        var result = service.UpdatePatient(id, new(request.FullName, request.DateOfBirth, request.Phone, request.Gender, request.Address), ParseActorId(User));
        return result is null ? NotFound() : Ok(result);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
