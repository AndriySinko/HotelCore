// This file contains code for UpdateMasterProjectCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.MastersProjects.Commands.UpdateMasterProject;

public class UpdateMasterProjectCommandHandler(
    IWorkerPortfolioItemRepository workRequestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMasterProjectCommand>
{
    public async Task Handle(
        UpdateMasterProjectCommand updateMasterProjectCommandrequest, 
        CancellationToken cancellationToken)
    {
         var masterProjectRequest = await workRequestRepository.GetByIdAsync(
             updateMasterProjectCommandrequest.MasterProjectId,
             cancellationToken)
             ?? throw new NotFoundException(nameof(WorkerPortfolioItem), updateMasterProjectCommandrequest.MasterProjectId);

        if (masterProjectRequest.WorkerProfile?.UserId != updateMasterProjectCommandrequest.CurrentUserId)
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }
        
         var changes = new Dictionary<string, (object? OldValue, object? NewValue)>();

        if (updateMasterProjectCommandrequest.Title != masterProjectRequest.Title)
        {
            changes[nameof(WorkerPortfolioItem.Title)] = (
                masterProjectRequest.Title,
                updateMasterProjectCommandrequest.Title);
            masterProjectRequest.Title = updateMasterProjectCommandrequest.Title;
        }

        if (updateMasterProjectCommandrequest.Description is not null &&
            updateMasterProjectCommandrequest.Description != masterProjectRequest.Description)
        {
            changes[nameof(WorkerPortfolioItem.Description)] = (
                masterProjectRequest.Description,
                updateMasterProjectCommandrequest.Description);
            masterProjectRequest.Description = updateMasterProjectCommandrequest.Description;
        }

        if (updateMasterProjectCommandrequest.CompletionDate.HasValue
            && updateMasterProjectCommandrequest.CompletionDate.Value != masterProjectRequest.CompletionDate)
        {
            changes[nameof(WorkerPortfolioItem.CompletionDate)] = (
                masterProjectRequest.CompletionDate,
                updateMasterProjectCommandrequest.CompletionDate.Value);
            masterProjectRequest.CompletionDate = updateMasterProjectCommandrequest.CompletionDate.Value;
        }
        
        if (changes.Count == 0)
            return;
         
        masterProjectRequest.UpdatedAt = DateTime.UtcNow ;
        
        workRequestRepository.Update(masterProjectRequest);
         
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
