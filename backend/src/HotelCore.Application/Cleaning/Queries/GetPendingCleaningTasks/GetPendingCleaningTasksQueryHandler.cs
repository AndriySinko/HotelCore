// This file contains code for GetPendingCleaningTasksQueryHandler.
using MediatR;
using HotelCore.Application.Cleaning.Queries.GetTasksForStaff;
using HotelCore.Application.Common.Interfaces.Cleaning;

namespace HotelCore.Application.Cleaning.Queries.GetPendingCleaningTasks;

public class GetPendingCleaningTasksQueryHandler(ICleaningTaskRepository cleaningTaskRepository)
    : IRequestHandler<GetPendingCleaningTasksQuery, List<CleaningTaskDto>>
{
    public async Task<List<CleaningTaskDto>> Handle(GetPendingCleaningTasksQuery query, CancellationToken ct)
    {
        var tasks = await cleaningTaskRepository.GetPendingTasksAsync(ct);

        return tasks
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.ScheduledDate)
            .Select(t => new CleaningTaskDto(
                t.Id,
                t.Room?.RoomNumber ?? string.Empty,
                t.RequestType.ToString(),
                t.Status.ToString(),
                t.ScheduledDate,
                t.Priority,
                t.AssignedStaffId,
                t.AssignedStaff?.UserName))
            .ToList();
    }
}
