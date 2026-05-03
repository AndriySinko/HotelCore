// This file contains code for HideMasterProjectCommand.
using MediatR;

namespace HotelCore.Application.MastersProjects.Commands.HideMasterProject;

public record HideMasterProjectCommand(
    Guid MasterProjectId,
    Guid CurrentUserId) : IRequest;
