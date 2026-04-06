using HotelCore.Application.Common.Models;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Requests;

public sealed record GetWorkRequestsListRequest : PageRequest
{
    public Guid? LocationId { get; init; }
    public DateTime? PreferredDate { get; init; }
    public decimal? MinBudget { get; init; }
    public decimal? MaxBudget { get; init; }
    public Guid[]? CategoryIds { get; init; }
    public Guid? SeekerUserId { get; init; }
    public WorkRequestStatus? Status { get; init; }
}
