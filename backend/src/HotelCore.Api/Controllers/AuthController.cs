using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Application.Identity;
using HotelCore.Application.Identity.Commands.ExternalLogin;
using HotelCore.Application.Identity.Commands.Login;
using HotelCore.Application.Identity.Commands.Register;
using HotelCore.Application.Identity.Commands.SwitchRole;
using HotelCore.Application.Identity.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using HotelCore.Application.Identity.Commands.ExternalLogin;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

public class AuthController(
    IMediator mediator, 
    IIdentityService identityService,
    IConfiguration configuration) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.Succeeded)
        {
            return UnauthorizedResult(result.Error ?? "Authentication failed");
        }

        return OkResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.Succeeded)
        {
            return BadRequestResult(result.Error ?? "Registration failed");
        }
        
        return OkResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await identityService.RefreshTokenAsync(
            request, 
            cancellationToken);
        
        if (!result.Succeeded)
        {
            return UnauthorizedResult(result.Error ?? "Token refresh failed");
        }
        
        return OkResult(result);
    }

    [HttpPost("switch-role")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> SwitchRole(
        [FromBody] SwitchRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.Succeeded)
        {
            return BadRequestResult(result.Error ?? "Role switch failed");
        }
        
        return OkResult(result);
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback))
        };
        
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        
        if (!result.Succeeded)
        {
            return UnauthorizedResult("External authentication failed");
        }

        var claims = result.Principal.Claims.ToList();
        
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
        {
            return BadRequestResult("Email claim not found");
        }

        var userInfo = new ExternalUserInfo(
            email,
            claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
            claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            claims.FirstOrDefault(c => c.Type == "picture")?.Value,
            nameof(AuthProvider.Google),
            claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? ""
        );

        var loginResult = await mediator.Send(new ExternalLoginCommand(userInfo));

        if (!loginResult.Succeeded)
        {
            return UnauthorizedResult(loginResult.Error ?? "External login failed");
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:5173";
        var callbackUrl = $"{frontendUrl}/auth/callback" +
            $"#token={Uri.EscapeDataString(loginResult.Token ?? "")}" +
            $"&refreshToken={Uri.EscapeDataString(loginResult.RefreshToken ?? "")}" +
            $"&userId={Uri.EscapeDataString(loginResult.UserId ?? "")}" +
            $"&userName={Uri.EscapeDataString(loginResult.UserName ?? "")}" +
            $"&role={Uri.EscapeDataString(loginResult.Role ?? "")}";  

        return Redirect(callbackUrl);
    }
}
