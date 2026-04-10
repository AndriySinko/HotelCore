// This file contains code for DeleteMasterProjectCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.MastersProjects.Commands.DeleteMasterProject;

public class DeleteMasterProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IWorkerPortfolioItemRepository workPortfolioItemRepository)
    : IRequestHandler<DeleteMasterProjectCommand>
{
public async Task Handle(
    DeleteMasterProjectCommand deleteMasterProjectCommand,
    CancellationToken cancellationToken)
    {
        var masterProject = await workPortfolioItemRepository.GetByIdAsync(
            deleteMasterProjectCommand.MasterProjectId,
            cancellationToken);

        if (masterProject is null)
        {
            throw new NotFoundException(nameof(MastersProjects), deleteMasterProjectCommand.MasterProjectId);
        }
        
        masterProject.DeletedAt = DateTime.UtcNow;
        
        workPortfolioItemRepository.Delete(masterProject);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}