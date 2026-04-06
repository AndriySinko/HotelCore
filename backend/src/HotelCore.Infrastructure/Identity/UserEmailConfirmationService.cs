using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Infrastructure.Identity;

public sealed class UserEmailConfirmationService(
    UserManager<User> userManager,
    ILogger<UserEmailConfirmationService> logger) : IUserEmailConfirmationService
{
    public async Task ConfirmAsync(string email, string? userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        User? user = null;
        if (Guid.TryParse(userId, out var parsedUserId))
        {
            user = await userManager.FindByIdAsync(parsedUserId.ToString());
        }

        user ??= await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            logger.LogWarning("Verified email {Email}, but no user account was found to confirm.", email);
            return;
        }

        if (user.EmailConfirmed)
        {
            return;
        }

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            logger.LogWarning("Failed to mark email as confirmed for user {UserId}: {Errors}", user.Id, errors);
        }
    }
}
