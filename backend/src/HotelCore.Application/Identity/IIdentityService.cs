using HotelCore.Application.Common.Models;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Identity;

public interface IIdentityService
{
    Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto);

    Task<AuthenticationResult> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> SwitchRoleAsync(Guid userId, UserRole targetRole, CancellationToken cancellationToken = default);

    Task<QrLoginResult> QrLoginAsync(string qrToken, CancellationToken cancellationToken = default);
}
