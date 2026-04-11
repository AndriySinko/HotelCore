// This file contains code for CleaningTask.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Cleaning;




public class CleaningTask : BaseEntity
{
    public Guid RoomId { get; set; }
    public Guid? ReservationId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public CleaningRequestType RequestType { get; set; }
    public CleaningTaskStatus Status { get; private set; } = CleaningTaskStatus.Requested;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public int Priority { get; set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CancellationReason { get; set; }

    
    public Room? Room { get; set; }
    public Reservation? Reservation { get; set; }
    public StaffMember? AssignedStaff { get; set; }

    public void SetStatus(CleaningTaskStatus status)
    {
        Status = status;
        if (status == CleaningTaskStatus.Completed)
            CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
