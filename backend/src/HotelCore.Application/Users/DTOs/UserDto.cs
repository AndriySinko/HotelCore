namespace HotelCore.Application.Users.DTOs;

using HotelCore.Application.Common.DTOs;
using HotelCore.Application.Profiles.DTOs;
using HotelCore.Domain.Enums;

public record UserDto(
    Guid Id,
    string Email,
    string PhoneNumber,
    string? FirstName,
    string? LastName,
    UserRole Role,
    bool IsEmailVerified,
    bool IsPhoneVerified,
    MyImageGroupDto? Avatar = null,
    SeekerProfileDto? SeekerProfile = null,
    WorkerProfileDto? WorkerProfile = null
);