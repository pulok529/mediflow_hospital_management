using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class PermissionsController(IIdentityService identityService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetPermissions() => Ok(identityService.GetPermissions());
}
