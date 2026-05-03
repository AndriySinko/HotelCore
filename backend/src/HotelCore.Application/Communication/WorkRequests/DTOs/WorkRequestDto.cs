// This file contains code for WorkRequestDto.
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.DTOs;

public class WorkRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SeekerProfileId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid LocationId { get; set; }
    public WorkRequestStatus Status { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? PreferredDate { get; set; }
    public string[] Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
