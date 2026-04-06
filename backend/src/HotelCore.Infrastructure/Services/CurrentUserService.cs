using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HotelCore.Application.Common.Interfaces;

namespace HotelCore.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue("uid") 
                             ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
