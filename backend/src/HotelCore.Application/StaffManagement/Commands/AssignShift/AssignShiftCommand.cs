// This file contains code for AssignShiftCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.StaffManagement.Commands.AssignShift;

public record AssignShiftCommand(
    Guid ScheduleId,
    Guid StaffMemberId,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    ShiftType ShiftType,
    string RequiredRole
) : IRequest<Guid>;
