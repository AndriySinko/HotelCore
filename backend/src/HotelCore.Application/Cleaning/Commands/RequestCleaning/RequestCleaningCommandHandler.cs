// This file contains code for RequestCleaningCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;

using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Cleaning.Commands.RequestCleaning;




public class RequestCleaningCommandHandler(
    ICleaningTaskRepository cleaningTaskRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<RequestCleaningCommand, Guid>
{
    public async Task<Guid> Handle(RequestCleaningCommand command, CancellationToken cancellationToken)
    {
        
        var existingTasksForRoom = await cleaningTaskRepository.GetByRoomAsync(command.RoomId, cancellationToken);
        bool hasActiveDuplicate = existingTasksForRoom.Any(existingTask =>
            existingTask.Status != CleaningTaskStatus.Completed &&
            existingTask.Status != CleaningTaskStatus.Cancelled &&
            existingTask.Status != CleaningTaskStatus.Rejected);

        if (hasActiveDuplicate)
            throw new ConflictException("An active cleaning task already exists for this room");

        var newCleaningTask = new CleaningTask
        {
            RoomId = command.RoomId,
            ReservationId = command.ReservationId,
            RequestType = command.RequestType,
            ScheduledDate = command.ScheduledDate,
            ScheduledTime = command.ScheduledTime,
            Priority = command.Priority
        };

        await cleaningTaskRepository.AddAsync(newCleaningTask, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newCleaningTask.Id;
    }
}
