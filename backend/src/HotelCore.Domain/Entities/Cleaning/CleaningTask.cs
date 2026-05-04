// represents a single cleaning job — created either by a guest request or automatically on checkout
// status flow: Requested → Assigned → InProgress → Completed → Verified (or Cancelled/Rejected)
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Cleaning;

public class CleaningTask : BaseEntity
{
    public Guid RoomId { get; set; }
    // reservation is optional — a deep clean or emergency clean may not be tied to any reservation
    public Guid? ReservationId { get; set; }
    // null until a supervisor assigns the task to a specific cleaning worker
    public Guid? AssignedStaffId { get; set; }
    // GuestRequest, DeepClean, Emergency, Turnover, Routine — determines urgency and procedure
    public CleaningRequestType RequestType { get; set; }
    // private setter — status changes go through SetStatus which also stamps CompletedAt if needed
    public CleaningTaskStatus Status { get; private set; } = CleaningTaskStatus.Requested;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ScheduledTime { get; set; }
    // 1 = Urgent, 2 = High, 3 = Normal, 4 = Low, 5 = No rush
    // lower number = higher urgency, tasks are sorted by this in GetTasksForStaff
    public int Priority { get; set; }
    // set automatically when status transitions to Completed
    public DateTime? CompletedAt { get; private set; }
    // required when the task is cancelled — explains why it was stopped
    public string? CancellationReason { get; set; }

    // navigation properties for related entities
    public Room? Room { get; set; }
    public Reservation? Reservation { get; set; }
    public StaffMember? AssignedStaff { get; set; }

    public void SetStatus(CleaningTaskStatus status)
    {
        Status = status;
        // record exactly when cleaning was finished — useful for reporting and SLA tracking
        if (status == CleaningTaskStatus.Completed)
            CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
