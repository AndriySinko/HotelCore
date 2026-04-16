// This file contains code for CompleteCleaningCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;

using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Cleaning.Commands.CompleteCleaning;



public class CompleteCleaningCommandHandler(
    ICleaningTaskRepository cleaningTaskRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CompleteCleaningCommand>
{
    public async Task Handle(CompleteCleaningCommand command, CancellationToken cancellationToken)
    {
        var cleaningTask = await cleaningTaskRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(CleaningTask), command.TaskId);

        if (cleaningTask.Status != CleaningTaskStatus.InProgress &&
            cleaningTask.Status != CleaningTaskStatus.Assigned)
            throw new BadRequestException("Only tasks in Assigned or InProgress state can be marked complete");

        cleaningTask.SetStatus(CleaningTaskStatus.Completed);
        cleaningTaskRepository.Update(cleaningTask);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
