using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Identity.Commands.SwitchRole;

public class SwitchRoleCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService,
    ILogger<SwitchRoleCommandHandler> logger)
    : IRequestHandler<SwitchRoleCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(
        SwitchRoleCommand request, 
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("SwitchRole failed: Unauthorized (no UserID claim)");
            return AuthenticationResult.Failure("Unauthorized");
        }

        logger.LogInformation("Processing role switch to {Role} for User {UserId}", request.TargetRole, userId);
        
        var parsedUserId = Guid.Parse(userId);
        var result = await identityService.SwitchRoleAsync(parsedUserId, request.TargetRole, cancellationToken);
        
        if (result.Succeeded)
        {
            logger.LogInformation("Successfully switched role for User {UserId}", userId);
        }
        else
        {
            logger.LogWarning("Failed to switch role for User {UserId}: {Error}", userId, result.Error);
        }

        return result;
    }
}
