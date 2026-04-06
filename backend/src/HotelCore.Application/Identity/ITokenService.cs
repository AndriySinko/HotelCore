using System.Security.Claims;
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Application.Identity;

public interface ITokenService
{
    string GenerateToken(User user, IList<string> roles);
    
    string GenerateRefreshToken();
    
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
