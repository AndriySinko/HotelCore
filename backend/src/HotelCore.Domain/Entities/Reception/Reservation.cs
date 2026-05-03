// central aggregate for the reception module — ties together a guest, a room, payments and services
// status flows: Reserved → CheckedIn → CheckedOut (or Cancelled at any point before checkout)
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Entities.Restaurant;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Reception;

public class Reservation : BaseEntity
{
    public Guid GuestId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }

    // private setter — all status changes must go through SetStatus so UpdatedAt is always stamped
    public ReservationStatus Status { get; private set; } = ReservationStatus.Reserved;

    // QR code is generated when the reservation is created and emailed to the guest
    // at check-in the receptionist can scan it or look up the reservation by guest name
    public string QrCode { get; set; } = string.Empty;
    // walk-in reservations are created on the spot at reception — no prior online booking
    public bool IsWalkIn { get; set; }

    // navigation properties — EF Core loads them only when explicitly included in the query
    public Guest? Guest { get; set; }
    public Room? Room { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<FoodOrder>? FoodOrders { get; set; }
    public ICollection<CleaningTask>? CleaningTasks { get; set; }

    public void SetStatus(ReservationStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    // calculates the total room charge — nights * price per night
    // payments and food orders are tracked separately, this is just the room cost
    public decimal GetTotalCharges()
    {
        var nights = (CheckOutDate - CheckInDate).Days;
        var roomRate = Room?.PricePerNight ?? 0;
        return nights * roomRate;
    }
}
