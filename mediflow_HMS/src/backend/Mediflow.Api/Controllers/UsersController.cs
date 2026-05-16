using ApiCreateUserRequest = Mediflow.Api.Contracts.Identity.CreateUserRequest;
using ApiUpdateUserRequest = Mediflow.Api.Contracts.Identity.UpdateUserRequest;
using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "SuperAdmin,Admin")]
public sealed class UsersController(IIdentityService identityService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(identityService.GetUsers());

    [HttpPost]
    public IActionResult Create([FromBody] ApiCreateUserRequest request)
    {
        var actorId = ParseActorId(User);
        var user = identityService.CreateUser(new(request.Email, request.Password, request.Roles), actorId);
        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] ApiUpdateUserRequest request)
    {
        var actorId = ParseActorId(User);
        var user = identityService.UpdateUser(id, new(request.Email, request.IsActive, request.Roles), actorId);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var actorId = ParseActorId(User);
        return identityService.DeleteUser(id, actorId) ? NoContent() : NotFound();
    }

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub"), out var id) ? id : null;
}
