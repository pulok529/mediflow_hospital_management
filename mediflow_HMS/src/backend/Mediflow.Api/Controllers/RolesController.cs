using ApiAssignPermissionsRequest = Mediflow.Api.Contracts.Identity.AssignPermissionsRequest;
using ApiCreateRoleRequest = Mediflow.Api.Contracts.Identity.CreateRoleRequest;
using ApiUpdateRoleRequest = Mediflow.Api.Contracts.Identity.UpdateRoleRequest;
using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class RolesController(IIdentityService identityService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(identityService.GetRoles());

    [HttpPost]
    public IActionResult Create([FromBody] ApiCreateRoleRequest request)
    {
        var actorId = ParseActorId(User);
        return Ok(identityService.CreateRole(new(request.Name), actorId));
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] ApiUpdateRoleRequest request)
    {
        var actorId = ParseActorId(User);
        var role = identityService.UpdateRole(id, new(request.Name), actorId);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var actorId = ParseActorId(User);
        return identityService.DeleteRole(id, actorId) ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/permissions")]
    public IActionResult AssignPermissions(Guid id, [FromBody] ApiAssignPermissionsRequest request)
    {
        var actorId = ParseActorId(User);
        var role = identityService.AssignPermissions(id, new(request.Permissions), actorId);
        return role is null ? NotFound() : Ok(role);
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub"), out var id) ? id : null;
}
