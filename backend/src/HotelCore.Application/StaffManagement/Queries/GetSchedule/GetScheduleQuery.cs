// This file contains code for GetScheduleQuery.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.StaffManagement.Queries.GetSchedule;

public record GetScheduleQuery(Guid ScheduleId) : IRequest<ScheduleDto?>;

public record ScheduleDto(
    Guid Id,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    string Status,
    List<ShiftDto> Shifts);

public record ShiftDto(
    Guid Id,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    string ShiftType,
    string RequiredRole,
    string Status,
    Guid? StaffMemberId,
    string? StaffName);
