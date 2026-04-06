using MediatR;
using Microsoft.AspNetCore.Http;
using HotelCore.Application.MastersProjects.DTOs;

namespace HotelCore.Application.MastersProjects.Commands.CreateMasterProject;

public record CreateMasterProjectCommand(
    Guid WorkerProfileId,
    string Title,
    string? Description,
    DateTime? CompletionDate,
    IReadOnlyList<IFormFile> Images
    ) : IRequest<MasterProjectDto>;
