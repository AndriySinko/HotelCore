// This file contains code for UpdateWorkRequestStatusCommandHandler.
using MediatR;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequestStatus;

public class UpdateWorkRequestStatusCommandHandler(
    IWorkRequestRepository workRequestRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWorkRequestStatusCommand>
{
    public async Task Handle(
        UpdateWorkRequestStatusCommand UpdateWorkRequestStatusCommandRequest,
        CancellationToken cancellationToken)
    {
        var workRequest = await workRequestRepository.GetByIdAsync(
                              UpdateWorkRequestStatusCommandRequest.WorkRequestId,
                              cancellationToken)
            ?? throw new NotFoundException(nameof(WorkRequest), UpdateWorkRequestStatusCommandRequest.WorkRequestId);

        if (workRequest.Status == UpdateWorkRequestStatusCommandRequest.NewStatus)
            return;

        var oldStatus = workRequest.Status;
        workRequest.Status = UpdateWorkRequestStatusCommandRequest.NewStatus;
        workRequest.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordPropertyChangeAsync<WorkRequest>(
            entityId: workRequest.Id,
            propertyName: nameof(WorkRequest.Status),
            oldValue: oldStatus,
            newValue: UpdateWorkRequestStatusCommandRequest.NewStatus,
           changedByUserId: UpdateWorkRequestStatusCommandRequest.CurrentUserId,
             $"Work request status changed from {oldStatus} " +
             $"to {UpdateWorkRequestStatusCommandRequest.NewStatus}");

        workRequestRepository.Update(workRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
