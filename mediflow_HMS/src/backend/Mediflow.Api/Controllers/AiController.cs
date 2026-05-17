using ApiAiApprovalDecision = Mediflow.Api.Contracts.AI.AiApprovalDecision;
using ApiAiDraftRequest = Mediflow.Api.Contracts.AI.AiDraftRequest;
using Mediflow.Application.Abstractions.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize(Roles = "SuperAdmin,Admin,Doctor,BillingOfficer,Pharmacist,LabTech,Radiologist,Nurse")]
public sealed class AiController(IAiWorkflowService service) : ControllerBase
{
    [HttpPost("drafts")]
    public IActionResult CreateDraft([FromBody] ApiAiDraftRequest request)
    {
        var result = service.CreateDraft(new(
            request.Feature,
            request.MinimalContext,
            request.PromptVersion,
            request.ModelVersion), ParseActorId(User));

        return Ok(result);
    }

    [HttpGet("drafts")]
    public IActionResult ListDrafts() => Ok(service.ListDrafts());

    [HttpGet("drafts/{requestId:guid}")]
    public IActionResult GetDraft(Guid requestId)
    {
        var result = service.GetDraft(requestId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("drafts/{requestId:guid}/review")]
    public IActionResult ReviewDraft(Guid requestId, [FromBody] ApiAiApprovalDecision decision)
    {
        var result = service.ReviewDraft(requestId, new(decision.Approved, decision.ReviewerNotes), ParseActorId(User));
        return result is null ? NotFound() : Ok(result);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
