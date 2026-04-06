using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Identity;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Entities.Seekers;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService,
    IConfiguration configuration,
    ApplicationDbContext dbContext) : IIdentityService
{
    public async Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto)
    {
        var systemRole = dto.Role switch
        {
            RegistrationRole.Worker => UserRole.Worker,
            RegistrationRole.Seeker => UserRole.Seeker,
            _ => UserRole.Seeker
        };

        var roleName = systemRole.ToString();
        
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = systemRole
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
        {
            return AuthenticationResult.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Create initial profiles based on role
        if (systemRole == UserRole.Worker)
        {
            user.WorkerProfile = new WorkerProfile
            {
                UserId = user.Id
            };
        }
        else
        {
            user.SeekerProfile = new SeekerProfile
            {
                UserId = user.Id
            };
        }

        await userManager.UpdateAsync(user);

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        await userManager.AddToRoleAsync(user, roleName);
        
        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthenticationResult> CreateUserAsync(CreateUserDto dto)
    {
        var roleName = dto.Role.ToString();
        
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role
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

        return AuthenticationResult.Success(null, null, user.Id.ToString(), user.UserName, roleName);
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

    public async Task<AuthenticationResult> ExternalLoginAsync(ExternalUserInfo userInfo)
    {
        var user = await userManager.FindByLoginAsync(
            userInfo.Provider, 
            userInfo.ProviderKey);

        if (user == null)
        {
            user = await userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    EmailConfirmed = true,
                    Role = UserRole.Seeker
                };

                var createResult = await userManager.CreateAsync(user);
                
                if (!createResult.Succeeded)
                {
                    return AuthenticationResult.Failure(string.Join(
                        ", ", 
                        createResult.Errors.Select(e => e.Description)));
                }

                user.SeekerProfile = new SeekerProfile
                {
                    UserId = user.Id
                };
                
                await userManager.UpdateAsync(user);

                var roleName = user.Role.ToString();
                
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
                
                await userManager.AddToRoleAsync(user, roleName);
            }

            var loginResult = await userManager.AddLoginAsync(
                user, 
                new UserLoginInfo(
                    userInfo.Provider, 
                    userInfo.ProviderKey, 
                    userInfo.Provider));
            
            if (!loginResult.Succeeded)
            {
                return AuthenticationResult.Failure(string.Join(
                    ", ", 
                    loginResult.Errors.Select(e => e.Description)));
            }
        }

        if (string.IsNullOrEmpty(user.FirstName))
        {
            user.FirstName = userInfo.FirstName;
        }
        
        if (string.IsNullOrEmpty(user.LastName))
        {
            user.LastName = userInfo.LastName;
        }

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        
        if (principal == null)
        {
            return AuthenticationResult.Failure("Invalid access token");
        }

        var userId = principal.FindFirst("uid")?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return AuthenticationResult.Failure("Invalid token claims");
        }

        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return AuthenticationResult.Failure("User not found");
        }

        if (user.RefreshToken != request.RefreshToken)
        {
            return AuthenticationResult.Failure("Invalid refresh token");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return AuthenticationResult.Failure("Refresh token expired");
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

        if (targetRole != UserRole.Worker && targetRole != UserRole.Seeker)
        {
            return AuthenticationResult.Failure("Invalid role switch request");
        }

        // Assign correct profile if it doesn't exist
        var profileNeedsCreation = false;
        if (targetRole == UserRole.Worker)
        {
            var exists = dbContext.Set<WorkerProfile>().Any(x => x.UserId == user.Id);
            if (!exists)
            {
                dbContext.Set<WorkerProfile>().Add(new WorkerProfile { UserId = user.Id });
                profileNeedsCreation = true;
            }
        }
        else if (targetRole == UserRole.Seeker)
        {
            var exists = dbContext.Set<SeekerProfile>().Any(x => x.UserId == user.Id);
            if (!exists)
            {
                dbContext.Set<SeekerProfile>().Add(new SeekerProfile { UserId = user.Id });
                profileNeedsCreation = true;
            }
        }

        if (profileNeedsCreation)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        user.Role = targetRole;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return AuthenticationResult.Failure("Failed to update user role");
        }

        // Manage ASP.NET Identity Roles
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var targetRoleName = targetRole.ToString();
        if (!await roleManager.RoleExistsAsync(targetRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(targetRoleName));
        }
        
        await userManager.AddToRoleAsync(user, targetRoleName);

        // Generate new token reflecting new role
        return await GenerateAuthResultAsync(user, cancellationToken);
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
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        
        var refreshTokenExpiryDays = int.Parse(configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");
        
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);
        
        await userManager.UpdateAsync(user);

        var userName = user.UserName ?? user.Email ?? "Unknown";

        return AuthenticationResult.Success(
            token, 
            refreshToken, 
            user.Id.ToString(), 
            userName, 
            role);
    }
}
