// a staff member uses this to request a shift change or swap with a colleague
// a manager reviews the request and either approves or rejects it with a comment
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.StaffManagement;

public class ShiftChangeRequest : BaseEntity
{
    public Guid ShiftId { get; set; }
    public Guid RequestedByStaffId { get; set; }
    // null until a manager reviews it
    public Guid? ReviewedByUserId { get; set; }
    // the employee must explain why they need the change
    public string Reason { get; set; } = string.Empty;
    // Pending → Approved or Rejected
    public ShiftChangeStatus Status { get; private set; } = ShiftChangeStatus.Pending;
    // optional — if the employee wants to swap shifts with a specific colleague
    public Guid? ProposedSwapPartnerId { get; set; }
    // manager can leave a note explaining the decision
    public string? ReviewerComment { get; set; }
    // set when the manager approves or rejects — tracks how long the request was open
    public DateTime? ReviewedAt { get; private set; }

    public Shift? Shift { get; set; }
    public StaffMember? RequestedBy { get; set; }
    public StaffMember? ReviewedBy { get; set; }

    public void SetStatus(ShiftChangeStatus status)
    {
        Status = status;
        // stamp the review time when a final decision is made
        if (status is ShiftChangeStatus.Approved or ShiftChangeStatus.Rejected)
            ReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
