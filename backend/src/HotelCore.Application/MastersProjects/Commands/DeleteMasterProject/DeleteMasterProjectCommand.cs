using MediatR;

namespace HotelCore.Application.MastersProjects.Commands.DeleteMasterProject;

public record DeleteMasterProjectCommand(
    Guid MasterProjectId) : IRequest;
   
