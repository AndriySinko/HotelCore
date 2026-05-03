// This file contains code for Reservation.
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

    
    public ReservationStatus Status { get; private set; } = ReservationStatus.Reserved;

    
    public string QrCode { get; set; } = string.Empty;
    public bool IsWalkIn { get; set; }

    
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

    
    
    
    
    public decimal GetTotalCharges()
    {
        var nights = (CheckOutDate - CheckInDate).Days;
        var roomRate = Room?.PricePerNight ?? 0;
        return nights * roomRate;
    }
}
