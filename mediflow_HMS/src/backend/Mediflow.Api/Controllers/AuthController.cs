using Mediflow.Api.Contracts.Auth;
using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var token = tokenService.CreateToken(Guid.NewGuid(), request.Email, ["dashboard:view"]);
        return Ok(new LoginResponse(token));
    }
}
