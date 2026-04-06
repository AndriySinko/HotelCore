using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Identity.Commands.ExternalLogin;

public class ExternalLoginCommandHandler(
    IIdentityService identityService,
    ILogger<ExternalLoginCommandHandler> logger)
    : IRequestHandler<ExternalLoginCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(
        ExternalLoginCommand request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting external login for {Email} via {Provider}", 
            request.UserInfo.Email, request.UserInfo.Provider);
        
        var result = await identityService.ExternalLoginAsync(request.UserInfo);

        if (result.Succeeded)
        {
            logger.LogInformation("External login successful for {Email} via {Provider}", 
                request.UserInfo.Email, request.UserInfo.Provider);
        }
        else
        {
            logger.LogWarning("External login failed for {Email} via {Provider}: {Error}", 
                request.UserInfo.Email, request.UserInfo.Provider, result.Error);
        }

        return result;
    }
}
