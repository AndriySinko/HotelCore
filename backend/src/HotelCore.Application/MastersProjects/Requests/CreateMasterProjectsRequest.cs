// This file contains code for CreateMasterProjectsRequest.
using Microsoft.AspNetCore.Http;

namespace HotelCore.Application.MastersProjects.Requests;

public record CreateMasterProjectsRequest()
{
    public Guid WorkerProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CompletionDate { get; set; }
    public IReadOnlyList<IFormFile> Images { get; set; } = [];
}
