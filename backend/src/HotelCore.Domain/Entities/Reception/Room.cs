// represents a physical hotel room
// status transitions: Available → Reserved → Occupied → UnderCleaning → Available
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Reception;

public class Room : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public int Floor { get; set; }
    // used to calculate payment amount at check-in
    public decimal PricePerNight { get; set; }

    // private setter — status changes must go through SetStatus to update the timestamp
    public RoomStatus Status { get; private set; } = RoomStatus.Available;

    public ICollection<Reservation>? Reservations { get; set; }
    public ICollection<CleaningTask>? CleaningTasks { get; set; }

    public void SetStatus(RoomStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    // convenience method used in CheckInCommandHandler to decide if an alternative room is needed
    public bool IsAvailable() => Status == RoomStatus.Available;
}
