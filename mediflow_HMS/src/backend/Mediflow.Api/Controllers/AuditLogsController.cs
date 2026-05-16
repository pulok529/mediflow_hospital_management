using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class AuditLogsController(IIdentityService identityService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAuditLogs() => Ok(identityService.GetAuditLogs());
}
