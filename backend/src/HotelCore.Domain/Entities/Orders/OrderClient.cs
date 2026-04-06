using HotelCore.Domain.Entities.Users;

namespace HotelCore.Domain.Entities.Orders;

/// <summary>
/// Represents private client information for an order.
/// Only visible to the master/company, not to other users.
/// </summary>
public class OrderClient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The order this client information belongs to.
    /// </summary>
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    /// <summary>
    /// Client's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Client's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Optional reference to a user if the client exists in the system.
    /// </summary>
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Access token for guest clients (when UserId is null).
    /// Allows non-registered clients to view their orders via a secure link.
    /// </summary>
    public string? GuestAccessToken { get; set; }
}
