using HotelCore.Application.Common.DTOs;

namespace HotelCore.Application.MastersProjects.DTOs;

public record MasterProjectListDto(
    Guid Id,
    Guid WorkerProfileId,
    string Title,
    DateTime? CompletionDate,
    IReadOnlyList<MyImageGroupDto> Images
    );
