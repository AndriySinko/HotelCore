// This file contains code for LoginCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Identity.Commands.Login;

public class LoginCommandHandler(
    IIdentityService identityService,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting login for user {Email}", request.Email);
        
        var result = await identityService.LoginAsync(new LoginUserDto(request.Email, request.Password));

        if (result.Succeeded)
        {
            logger.LogInformation("Login successful for user {Email}", request.Email);
        }
        else
        {
            logger.LogWarning("Login failed for user {Email}: {Error}", request.Email, result.Error);
        }

        return result;
    }
}
