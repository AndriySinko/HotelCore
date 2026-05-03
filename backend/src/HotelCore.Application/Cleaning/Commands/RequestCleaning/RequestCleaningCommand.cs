// This file contains code for RequestCleaningCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Cleaning.Commands.RequestCleaning;

public record RequestCleaningCommand(
    Guid RoomId,
    Guid? ReservationId,
    CleaningRequestType RequestType,
    DateTime ScheduledDate,
    DateTime? ScheduledTime,
    int Priority
) : IRequest<Guid>;
