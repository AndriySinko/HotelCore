using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Entities.Companies;
using HotelCore.Domain.Entities.Seekers;
using Microsoft.AspNetCore.Identity;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity
{
    public UserRole Role { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public Guid? AvatarId { get; set; }
    public MyImageGroup? Avatar { get; set; }
    
    public WorkerProfile? WorkerProfile { get; set; }
    public SeekerProfile? SeekerProfile { get; set; }
    public Company? Company { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    #region IBaseEntity implementation

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    #endregion
}
