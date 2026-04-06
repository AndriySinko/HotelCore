namespace HotelCore.Application.Profiles.DTOs;

public record SeekerProfileDto(
    Guid Id,
    string? Bio,
    decimal Rating,
    int ReviewsCount,
    bool IsVerified,
    Guid? DefaultLocationId
);

public record WorkerProfileDto(
    Guid Id,
    string? Bio,
    string? Website,
    decimal Rating,
    int ReviewsCount,
    bool IsVerified,
    string[] Tags,
    Guid? LocationId
);
