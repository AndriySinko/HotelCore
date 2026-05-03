// This file contains code for Shift.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class Shift : BaseEntity
{
    public Guid WorkScheduleId { get; set; }
    public Guid? StaffMemberId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ShiftType ShiftType { get; set; }
    public string RequiredRole { get; set; } = string.Empty;
    public ShiftStatus Status { get; private set; } = ShiftStatus.Uncovered;

    
    public WorkSchedule? WorkSchedule { get; set; }
    public StaffMember? AssignedEmployee { get; set; }

    public void SetStatus(ShiftStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    
    public decimal GetDurationHours() => (decimal)(EndTime - StartTime).TotalHours;
}
