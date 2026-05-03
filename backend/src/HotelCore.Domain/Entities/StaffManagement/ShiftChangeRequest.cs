// This file contains code for ShiftChangeRequest.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class ShiftChangeRequest : BaseEntity
{
    public Guid ShiftId { get; set; }
    public Guid RequestedByStaffId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ShiftChangeStatus Status { get; private set; } = ShiftChangeStatus.Pending;
    public Guid? ProposedSwapPartnerId { get; set; }
    public string? ReviewerComment { get; set; }
    public DateTime? ReviewedAt { get; private set; }

    
    public Shift? Shift { get; set; }
    public StaffMember? RequestedBy { get; set; }
    public StaffMember? ReviewedBy { get; set; }

    public void SetStatus(ShiftChangeStatus status)
    {
        Status = status;
        if (status is ShiftChangeStatus.Approved or ShiftChangeStatus.Rejected)
            ReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
