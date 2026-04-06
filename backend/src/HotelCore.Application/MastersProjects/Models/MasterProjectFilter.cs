namespace HotelCore.Application.MastersProjects.Models;

public record MasterProjectFilter(
    Guid? WorkerProfileId,
    string? Search);
