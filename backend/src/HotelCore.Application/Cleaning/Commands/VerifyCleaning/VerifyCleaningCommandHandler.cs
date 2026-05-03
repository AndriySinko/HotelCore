// This file contains code for VerifyCleaningCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Cleaning.Commands.VerifyCleaning;



public class VerifyCleaningCommandHandler(
    ICleaningTaskRepository cleaningTaskRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<VerifyCleaningCommand>
{
    public async Task Handle(VerifyCleaningCommand command, CancellationToken cancellationToken)
    {
        var cleaningTask = await cleaningTaskRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(CleaningTask), command.TaskId);

        
        
        if (cleaningTask.Status == CleaningTaskStatus.Verified)
            throw new BadRequestException("Cleaning task has already been verified");

        if (cleaningTask.Status == CleaningTaskStatus.Cancelled || cleaningTask.Status == CleaningTaskStatus.Rejected)
            throw new BadRequestException(
                $"Cannot verify a task that is already {cleaningTask.Status}");

        if (cleaningTask.Status != CleaningTaskStatus.Completed)
            throw new BadRequestException(
                $"Cannot verify a task with status '{cleaningTask.Status}' — task must be Completed first");

        cleaningTask.SetStatus(CleaningTaskStatus.Verified);
        cleaningTaskRepository.Update(cleaningTask);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
