using HotelCore.Application.Common.Models;

namespace HotelCore.Application.MastersProjects.Requests;

public sealed record GetMasterProjectListRequest : PageRequest
{
    public Guid? WorkerProfileId { get; init; }
    public string? Search { get; init; }
}
