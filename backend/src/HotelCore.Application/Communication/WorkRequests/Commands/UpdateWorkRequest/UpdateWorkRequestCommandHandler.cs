using MediatR;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequest;

public class UpdateWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWorkRequestCommand>
{
    public async Task Handle(
        UpdateWorkRequestCommand updateWorkRequestCommandRequest,
        CancellationToken cancellationToken)
    {
        var workRequest = await workRequestRepository.GetByIdAsync(
                              updateWorkRequestCommandRequest.WorkRequestId, 
                              cancellationToken)
            ?? throw new NotFoundException(
                nameof(WorkRequest), 
                updateWorkRequestCommandRequest.WorkRequestId);

        var changes = new Dictionary<string, (object? OldValue, object? NewValue)>();

        if (updateWorkRequestCommandRequest.Title is not null
            && updateWorkRequestCommandRequest.Title != workRequest.Title)
        {
            changes[nameof(WorkRequest.Title)] = (
                workRequest.Title,
                updateWorkRequestCommandRequest.Title);
            workRequest.Title = updateWorkRequestCommandRequest.Title;
        }

        if (updateWorkRequestCommandRequest.Description is not null &&
            updateWorkRequestCommandRequest.Description != workRequest.Description)
        {
            changes[nameof(WorkRequest.Description)] = (
                workRequest.Description,
                updateWorkRequestCommandRequest.Description);
            workRequest.Description = updateWorkRequestCommandRequest.Description;
        }

        if (updateWorkRequestCommandRequest.CategoryId.HasValue
            && updateWorkRequestCommandRequest.CategoryId.Value != workRequest.CategoryId)
        {
            changes[nameof(WorkRequest.CategoryId)] = (
                workRequest.CategoryId,
                updateWorkRequestCommandRequest.CategoryId.Value);
            workRequest.CategoryId = updateWorkRequestCommandRequest.CategoryId.Value;
        }

        if (updateWorkRequestCommandRequest.LocationId.HasValue
            && updateWorkRequestCommandRequest.LocationId.Value != workRequest.LocationId)
        {
            changes[nameof(WorkRequest.LocationId)] = (
                workRequest.LocationId,
                updateWorkRequestCommandRequest.LocationId.Value);
            workRequest.LocationId = updateWorkRequestCommandRequest.LocationId.Value;
        }

        if (updateWorkRequestCommandRequest.Budget != workRequest.Budget)
        {
            changes[nameof(WorkRequest.Budget)] = (
                workRequest.Budget,
                updateWorkRequestCommandRequest.Budget);
            workRequest.Budget = updateWorkRequestCommandRequest.Budget;
        }

        if (updateWorkRequestCommandRequest.PreferredDate != workRequest.PreferredDate)
        {
            changes[nameof(WorkRequest.PreferredDate)] = (
                workRequest.PreferredDate,
                updateWorkRequestCommandRequest.PreferredDate);
            workRequest.PreferredDate = updateWorkRequestCommandRequest.PreferredDate;
        }

        if (updateWorkRequestCommandRequest.Tags is not null 
            && !updateWorkRequestCommandRequest.Tags.SequenceEqual(workRequest.Tags))
        {
            changes[nameof(WorkRequest.Tags)] = (
                workRequest.Tags, 
                updateWorkRequestCommandRequest.Tags);
            workRequest.Tags = updateWorkRequestCommandRequest.Tags;
        }

        if (changes.Count == 0)
            return;

        workRequest.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordChangesAsync<WorkRequest>(
            entityId: workRequest.Id,
            changes: changes,
            changedByUserId: updateWorkRequestCommandRequest.CurrentUserId,
            changeDescription: "Work updateWorkRequestCommandRequest updated");

        workRequestRepository.Update(workRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
