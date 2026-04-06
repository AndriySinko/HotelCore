using HotelCore.Domain.Enums;

namespace HotelCore.Application.Identity.DTOs;

public record RegisterUserDto(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName,
    RegistrationRole Role
);
