using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Identity.Commands.Register;

public class RegisterCommandHandler(
    IIdentityService identityService,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(
        RegisterCommand request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing registration request for {Email}", request.Email);
        
        var result = await identityService.RegisterAsync(new RegisterUserDto(
            request.Email, 
            request.Password, 
            request.FirstName, 
            request.LastName,
            request.Role));

        if (result.Succeeded)
        {
            logger.LogInformation("User {Email} registered successfully", request.Email);
        }
        else
        {
            logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.Error);
        }

        return result;
    }
}
