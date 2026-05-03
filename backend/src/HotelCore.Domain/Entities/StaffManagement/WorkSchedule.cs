// This file contains code for WorkSchedule.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class WorkSchedule : BaseEntity
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ScheduleStatus Status { get; private set; } = ScheduleStatus.Draft;
    public Guid CreatedByUserId { get; set; }
    public DateTime? PublishedAt { get; private set; }

    
    public StaffMember? CreatedBy { get; set; }
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public void SetStatus(ScheduleStatus status)
    {
        Status = status;
        if (status == ScheduleStatus.Published)
            PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
