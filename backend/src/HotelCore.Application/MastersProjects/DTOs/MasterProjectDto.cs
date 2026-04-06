using HotelCore.Application.Common.DTOs;

namespace HotelCore.Application.MastersProjects.DTOs;

public record MasterProjectDto(
    Guid Id,
    Guid WorkerProfileId,
    string Title,
    string? Description,
    DateTime? CompletionDate,
    IReadOnlyList<MyImageGroupDto> Images
);
