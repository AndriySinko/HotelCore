// This file contains code for DeleteWorkRequestCommandHandler.
using MediatR;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Communication.WorkRequests.Commands.DeleteWorkRequest;

public class DeleteWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteWorkRequestCommand>
{
    public async Task Handle(DeleteWorkRequestCommand deleteWorkRequestCommandRequest, CancellationToken cancellationToken)
    {
        var workRequest = await workRequestRepository.GetByIdAsync(deleteWorkRequestCommandRequest.WorkRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkRequest), deleteWorkRequestCommandRequest.WorkRequestId);

        if (workRequest.IsDeleted)
            return;

        workRequest.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordPropertyChangeAsync<WorkRequest>(
            entityId: workRequest.Id,
            propertyName: nameof(WorkRequest.IsDeleted),
            oldValue: false,
            newValue: true,
            changedByUserId: deleteWorkRequestCommandRequest.CurrentUserId,
            changeDescription: "Work request deleted");

        workRequestRepository.Delete(workRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
