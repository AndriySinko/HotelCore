using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Application.Identity;
using HotelCore.Application.Identity.Commands.Login;
using HotelCore.Application.Identity.Commands.Register;
using HotelCore.Application.Identity.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Succeeded)
        {
            return Unauthorized(result.Error ?? "Authentication failed");
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Error ?? "Registration failed");
        }
        
        return Ok(result);
    }
}
