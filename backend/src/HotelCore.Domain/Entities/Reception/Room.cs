// This file contains code for Room.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Reception;






public class Room : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public int Floor { get; set; }
    public decimal PricePerNight { get; set; }

    
    public RoomStatus Status { get; private set; } = RoomStatus.Available;

    
    public ICollection<Reservation>? Reservations { get; set; }
    public ICollection<CleaningTask>? CleaningTasks { get; set; }

    public void SetStatus(RoomStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    
    public bool IsAvailable() => Status == RoomStatus.Available;
}
