namespace HotelCore.Application.Identity.DTOs;

public record ExternalUserInfo(
    string Email,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string Provider,
    string ProviderKey
);
