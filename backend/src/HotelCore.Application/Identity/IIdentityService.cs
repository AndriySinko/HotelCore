using HotelCore.Application.Common.Models;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Identity;

public interface IIdentityService
{
    Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto);
    
    Task<AuthenticationResult> CreateUserAsync(CreateUserDto dto);

    Task<AuthenticationResult> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    
    Task<AuthenticationResult> ExternalLoginAsync(ExternalUserInfo userInfo);
    
    Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> SwitchRoleAsync(Guid userId, UserRole targetRole, CancellationToken cancellationToken = default);
}
