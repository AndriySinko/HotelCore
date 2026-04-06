namespace HotelCore.Domain.Enums;

/// <summary>
/// Roles available for a user during self-registration.
/// Limits exposure of administrative roles to the public API.
/// </summary>
public enum RegistrationRole
{
    Seeker = 1,
    Worker = 2
}
