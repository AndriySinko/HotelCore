// This file contains code for UpdateMasterProjectCommand.
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HotelCore.Application.MastersProjects.Commands.UpdateMasterProject;

public record UpdateMasterProjectCommand(
    Guid MasterProjectId,
    Guid CurrentUserId,
    string Title,
    string? Description,
    DateTime? CompletionDate,
    IReadOnlyList<IFormFile> Images) : IRequest;
