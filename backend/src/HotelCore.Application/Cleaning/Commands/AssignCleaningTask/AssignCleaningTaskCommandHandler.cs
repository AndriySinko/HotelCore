// This file contains code for AssignCleaningTaskCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Cleaning.Commands.AssignCleaningTask;

public class AssignCleaningTaskCommandHandler(
    ICleaningTaskRepository cleaningTaskRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignCleaningTaskCommand>
{
    public async Task Handle(AssignCleaningTaskCommand command, CancellationToken ct)
    {
        var task = await cleaningTaskRepository.GetByIdAsync(command.TaskId, ct)
            ?? throw new NotFoundException(nameof(CleaningTask), command.TaskId);

        if (task.Status != CleaningTaskStatus.Requested)
            throw new BadRequestException($"Cannot assign a task with status '{task.Status}'");

        task.AssignedStaffId = command.StaffId;
        task.SetStatus(CleaningTaskStatus.Assigned);
        cleaningTaskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
