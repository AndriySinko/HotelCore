// This file contains code for PublishScheduleCommand.
using MediatR;

namespace HotelCore.Application.StaffManagement.Commands.PublishSchedule;

public record PublishScheduleCommand(Guid ScheduleId) : IRequest;
