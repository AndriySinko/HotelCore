using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.DTOs;

public class SimpleWorkRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public WorkRequestStatus Status { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? PreferredDate { get; set; }
    public Guid CategoryId { get; set; }
    public Guid LocationId { get; set; }
    public Guid SeekerProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
}
