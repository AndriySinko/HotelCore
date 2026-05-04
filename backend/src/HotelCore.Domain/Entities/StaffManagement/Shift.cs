// one shift slot inside a work schedule — can be unassigned (Uncovered) or assigned to a staff member
// StartTime and EndTime are full DateTimes not TimeSpan — makes midnight crossing easier to handle
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class Shift : BaseEntity
{
    // which schedule this shift belongs to
    public Guid WorkScheduleId { get; set; }
    // null until someone is assigned — status is Uncovered in that case
    public Guid? StaffMemberId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    // Morning / Afternoon / Night — used for display and filtering in the frontend
    public ShiftType ShiftType { get; set; }
    // the role required for this slot, e.g. "CleaningWorker" or "Receptionist"
    public string RequiredRole { get; set; } = string.Empty;
    // Uncovered when no one is assigned, Assigned once a staff member is linked
    public ShiftStatus Status { get; private set; } = ShiftStatus.Uncovered;

    public WorkSchedule? WorkSchedule { get; set; }
    public StaffMember? AssignedEmployee { get; set; }

    public void SetStatus(ShiftStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    // used by AssignShiftCommandHandler to sum up weekly hours and check against the contract limit
    public decimal GetDurationHours() => (decimal)(EndTime - StartTime).TotalHours;
}
