// base user entity — shared by Guest and StaffMember
// extends ASP.NET Identity so we get built-in password hashing, login, and role management for free
// IBaseEntity adds soft-delete support — deleted users are flagged, not removed from the database
using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity
{
    // our own role enum — used for authorization checks across the app
    // ASP.NET Identity also has its own roles table but we use this field for simplicity
    public UserRole Role { get; set; }

    #region IBaseEntity implementation

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    // when a user is "deleted" we just set IsDeleted = true and record the time
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    #endregion
}
