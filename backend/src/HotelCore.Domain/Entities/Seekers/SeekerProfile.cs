using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Entities.Locations;

namespace HotelCore.Domain.Entities.Seekers;

/// <summary>
/// Profile for a seeker (client) who is looking for specialists or posting work requests.
/// </summary>
public class SeekerProfile : BaseEntity
{
    /// <summary>
    /// Link to the user who owns this profile.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// Brief description of the seeker's needs or background.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Aggregate rating as a customer (based on master reviews).
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Total count of reviews received from masters.
    /// </summary>
    public int ReviewsCount { get; set; }

    /// <summary>
    /// Indicates if the seeker has passed identity verification.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Preferred location for services (if any).
    /// </summary>
    public Guid? DefaultLocationId { get; set; }
    public Location? DefaultLocation { get; set; }
}
