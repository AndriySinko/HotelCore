using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Entities.Locations;

namespace HotelCore.Domain.Entities.Workers;

/// <summary>
/// A proactive listing created by a worker to promote their specific services to seekers.
/// Example: "Expert Plumbing in Bratislava", "Immediate Electrical Repairs".
/// </summary>
public class WorkerServiceListing : BaseEntity
{
    /// <summary>
    /// Title of the service listing.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of what is offered.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Worker profile that owns this listing.
    /// </summary>
    public Guid WorkerProfileId { get; set; }
    public WorkerProfile WorkerProfile { get; set; } = null!;

    /// <summary>
    /// Category of the service.
    /// </summary>
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Base price for the service (optional).
    /// </summary>
    public decimal? StartingPrice { get; set; }

    /// <summary>
    /// Primary location for this service.
    /// </summary>
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    /// <summary>
    /// Searchable tags for this specific listing.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// Indicates if the listing is currently active and visible to seekers.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
