using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Identity;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace HotelCore.Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService,
    ApplicationDbContext db,
    ILogger<IdentityService> logger) : IIdentityService
{
    private const string DemoGuestEmail = "demo@hotelcore.local";
    private const string DemoGuestPassword = "Demo@12345!";

    public async Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto)
    {
        var systemRole = dto.Role;

        var roleName = systemRole.ToString();

        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            Role = systemRole
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return AuthenticationResult.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        await userManager.AddToRoleAsync(user, roleName);

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthenticationResult> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return AuthenticationResult.Failure("Invalid email or password.");
        }

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            return AuthenticationResult.Failure("Invalid email or password.");
        }

        return await GenerateAuthResultAsync(user, cancellationToken);
    }

    public async Task<AuthenticationResult> SwitchRoleAsync(Guid userId, UserRole targetRole, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return AuthenticationResult.Failure("User not found");
        }

        if (targetRole == UserRole.Guest)
        {
            return AuthenticationResult.Failure("Invalid role switch request");
        }

        user.Role = targetRole;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return AuthenticationResult.Failure("Failed to update user role");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var targetRoleName = targetRole.ToString();
        if (!await roleManager.RoleExistsAsync(targetRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(targetRoleName));
        }

        await userManager.AddToRoleAsync(user, targetRoleName);

        return await GenerateAuthResultAsync(user, cancellationToken);
    }

    public async Task<QrLoginResult> QrLoginAsync(string qrToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(qrToken))
            return QrLoginResult.Failure("Invalid QR code.");

        if (qrToken.Equals("demo", StringComparison.OrdinalIgnoreCase))
            return await HandleDemoLoginAsync();

        var code = ExtractReservationCode(qrToken);

        var reservation = await db.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.QrCode == code, cancellationToken);

        if (reservation is null)
            return QrLoginResult.Failure("QR code not recognised.");

        if (reservation.Guest is null)
            return QrLoginResult.Failure("No guest linked to this reservation.");

        var token = tokenService.GenerateToken(reservation.Guest, [UserRole.Guest.ToString()]);

        return QrLoginResult.Success(
            token,
            reservation.Guest.Id.ToString(),
            reservation.Guest.GetFullName(),
            reservation.Room?.RoomNumber);
    }

    // Extracts "HC-C7EAHC" from either "http://localhost:3000/reservation/HC-C7EAHC" or a bare code.
    private static string ExtractReservationCode(string qrToken)
    {
        if (Uri.TryCreate(qrToken, UriKind.Absolute, out var uri))
            return uri.Segments.Last().Trim('/');

        return qrToken.Trim();
    }

    private async Task<QrLoginResult> HandleDemoLoginAsync()
    {
        var user = await userManager.FindByEmailAsync(DemoGuestEmail);

        if (user is null)
        {
            var guest = new Guest
            {
                Email = DemoGuestEmail,
                UserName = DemoGuestEmail,
                FirstName = "George",
                LastName = "Sladkovsky",
                RoomNumber = "404",
                Role = UserRole.Guest,
            };

            var createResult = await userManager.CreateAsync(guest, DemoGuestPassword);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to seed demo guest: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return QrLoginResult.Failure("Could not provision demo guest.");
            }

            user = guest;
        }

        var token = tokenService.GenerateToken(user, [UserRole.Guest.ToString()]);
        var demoGuest = user as Guest;
        var name = demoGuest is not null
            ? $"{demoGuest.FirstName} {demoGuest.LastName}"
            : user.UserName ?? DemoGuestEmail;

        return QrLoginResult.Success(token, user.Id.ToString(), name, demoGuest?.RoomNumber);
    }

    private async Task<AuthenticationResult> GenerateAuthResultAsync(User user, CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(user);

        if (roles.Count == 0)
        {
            var roleName = user.Role.ToString();

            if (!string.Equals(roleName, UserRole.Guest.ToString(), StringComparison.Ordinal))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }

                var addToRoleResult = await userManager.AddToRoleAsync(user, roleName);
                if (addToRoleResult.Succeeded)
                {
                    roles = await userManager.GetRolesAsync(user);
                }
                else
                {
                    roles = new List<string> { roleName };
                }
            }
        }

        var role = roles.FirstOrDefault() ?? user.Role.ToString();

        var token = tokenService.GenerateToken(user, roles);

        var userName = user.UserName ?? user.Email ?? "Unknown";

        return AuthenticationResult.Success(
            token,
            user.Id.ToString(),
            userName,
            role);
    }
}
