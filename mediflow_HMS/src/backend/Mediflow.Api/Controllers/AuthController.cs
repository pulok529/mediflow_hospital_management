using Mediflow.Api.Contracts.Auth;
using Mediflow.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var tokens = identityService.Login(request.Email, request.Password);
        return Ok(new AuthResponse(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAtUtc, tokens.RefreshTokenExpiresAtUtc));
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public ActionResult<AuthResponse> Refresh([FromBody] RefreshRequest request)
    {
        var tokens = identityService.Refresh(request.RefreshToken);
        return Ok(new AuthResponse(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAtUtc, tokens.RefreshTokenExpiresAtUtc));
    }
}
