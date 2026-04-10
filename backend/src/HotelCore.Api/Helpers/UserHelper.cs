// This file contains code for UserHelper.
using System.Security.Claims;

namespace HotelCore.Api.Helpers;

public static class UserHelper
{
    public static string? GetUserId(HttpContext? httpContext)
        => httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static Guid? GetUserIdGuid(HttpContext? httpContext)
        => Guid.TryParse(GetUserId(httpContext), out var id) ? id : null;

    public static Guid GetUserIdOrThrow(HttpContext? httpContext)
        => GetUserIdGuid(httpContext) 
           ?? throw new UnauthorizedAccessException("User ID not found in claims");
}
