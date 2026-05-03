// This file contains code for User.
using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity
{
    public UserRole Role { get; set; }

    #region IBaseEntity implementation

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    #endregion
}
