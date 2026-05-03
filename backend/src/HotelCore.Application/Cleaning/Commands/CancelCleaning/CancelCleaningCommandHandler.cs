// This file contains code for CancelCleaningCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;


namespace HotelCore.Application.Cleaning.Commands.CancelCleaning;

public class CancelCleaningCommandHandler(
    ICleaningTaskRepository cleaningTaskRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CancelCleaningCommand>
{
    public async Task Handle(CancelCleaningCommand command, CancellationToken cancellationToken)
    {
        var cleaningTask = await cleaningTaskRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(CleaningTask), command.TaskId);

        if (cleaningTask.Status == CleaningTaskStatus.Completed ||
            cleaningTask.Status == CleaningTaskStatus.Verified)
            throw new BadRequestException("Cannot cancel a task that is already completed or verified");

        cleaningTask.CancellationReason = command.CancellationReason;
        cleaningTask.SetStatus(CleaningTaskStatus.Cancelled);
        cleaningTaskRepository.Update(cleaningTask);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
