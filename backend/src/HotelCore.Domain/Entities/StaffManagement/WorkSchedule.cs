// a work schedule covers one week and contains a list of shifts assigned to staff
// starts as Draft so the manager can build it before making it visible to employees
// status: Draft → Published (cant go back once published)
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class WorkSchedule : BaseEntity
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    // tracks current state — only Published schedules are visible to staff
    public ScheduleStatus Status { get; private set; } = ScheduleStatus.Draft;
    public Guid CreatedByUserId { get; set; }
    // set when the manager publishes — shows staff when they can start seeing their shifts
    public DateTime? PublishedAt { get; private set; }

    public StaffMember? CreatedBy { get; set; }
    // all the individual shift slots for this period
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public void SetStatus(ScheduleStatus status)
    {
        Status = status;
        if (status == ScheduleStatus.Published)
            PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
