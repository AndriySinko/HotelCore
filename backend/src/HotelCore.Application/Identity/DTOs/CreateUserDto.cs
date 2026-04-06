using HotelCore.Domain.Enums;

namespace HotelCore.Application.Identity.DTOs;

public record CreateUserDto(
    string Email, 
    string Password, 
    UserRole Role, 
    string? FirstName, 
    string? LastName, 
    string? PhoneNumber
);
