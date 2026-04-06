using MediatR;

namespace HotelCore.Application.MastersProjects.Commands.UnhideMasterProject;

public record UnhideMasterProjectCommand(
    Guid MasterProjectId,
    Guid CurrentUserId) : IRequest;
