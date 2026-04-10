// This file contains code for HideMasterProjectCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.MastersProjects.Commands.HideMasterProject;

public class HideMasterProjectCommandHandler(
        IWorkerPortfolioItemRepository workerPortfolioItemRepository,
        IUnitOfWork unitOfWork
) : IRequestHandler<HideMasterProjectCommand>
{
    public async Task Handle(
        HideMasterProjectCommand hideMasterProjectCommandrequest,
        CancellationToken cancellationToken)
    {
        var masterProject = await workerPortfolioItemRepository.GetByIdAsync(
            hideMasterProjectCommandrequest.MasterProjectId,
            cancellationToken)
            ?? throw new NotFoundException(nameof(WorkerPortfolioItem), hideMasterProjectCommandrequest.MasterProjectId);
        
        if (masterProject.WorkerProfile.UserId != hideMasterProjectCommandrequest.CurrentUserId)
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }

        if (masterProject.IsHidden)
        {
            return;
        }

        masterProject.IsHidden = true;
        
        masterProject.UpdatedAt = DateTime.UtcNow;
        
        workerPortfolioItemRepository.Update(masterProject);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
