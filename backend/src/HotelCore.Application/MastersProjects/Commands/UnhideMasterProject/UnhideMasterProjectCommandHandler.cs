// This file contains code for UnhideMasterProjectCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;

using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.MastersProjects.Commands.UnhideMasterProject;

public class UnhideMasterProjectCommandHandler(
    IWorkerPortfolioItemRepository workerPortfolioItemRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UnhideMasterProjectCommand>
{
    public async Task Handle(
        UnhideMasterProjectCommand unhideMasterProjectCommandRequest,
        CancellationToken cancellationToken)
    {
        var masterProject = await workerPortfolioItemRepository.GetByIdAsync(
            unhideMasterProjectCommandRequest.MasterProjectId,
            cancellationToken)
            ?? throw new NotFoundException(nameof(WorkerPortfolioItem), unhideMasterProjectCommandRequest.MasterProjectId);

        if (masterProject.WorkerProfile?.UserId != unhideMasterProjectCommandRequest.CurrentUserId)
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }

        if (!masterProject.IsHidden)
        {
            return;
        }
        
        masterProject.IsHidden = false;
        
        masterProject.UpdatedAt = DateTime.UtcNow;
        
        workerPortfolioItemRepository.Update(masterProject);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
