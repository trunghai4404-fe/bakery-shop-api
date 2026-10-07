using System.Reflection.Metadata;
using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.RefreshToken;
using Identity.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;
[ApiController]
[Route("api/v1/auth")]
public class AuthController(ISender sender) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }
}