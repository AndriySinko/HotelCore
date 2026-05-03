// This file contains code for CreateScheduleCommand.
using MediatR;

namespace HotelCore.Application.StaffManagement.Commands.CreateSchedule;

public record CreateScheduleCommand(
    DateTime PeriodStart,
    DateTime PeriodEnd,
    Guid CreatedByUserId
) : IRequest<Guid>;
