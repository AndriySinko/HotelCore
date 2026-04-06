namespace HotelCore.Application.Users.DTOs;

using HotelCore.Application.Common.DTOs;

public record PublicProfileDto(
    Guid Id,
    string? FirstName,
    string? LastName,
    DateTime CreatedAt,
    MyImageGroupDto? Avatar,
    int ReviewsCount,
    int CompletedOrdersCount,
    int CreatedOrdersCount,
    decimal Rating
);
