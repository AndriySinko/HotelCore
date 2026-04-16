// This file contains code for GetTasksForStaffQuery.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Cleaning.Queries.GetTasksForStaff;

public record GetTasksForStaffQuery(Guid StaffId) : IRequest<List<CleaningTaskDto>>;

public record CleaningTaskDto(
    Guid TaskId,
    string RoomNumber,
    string RequestType,
    string Status,
    DateTime ScheduledDate,
    int Priority,
    Guid? AssignedStaffId = null,
    string? AssignedStaffName = null);
