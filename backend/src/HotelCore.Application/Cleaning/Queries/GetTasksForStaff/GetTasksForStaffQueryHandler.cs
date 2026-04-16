// This file contains code for GetTasksForStaffQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Cleaning;

namespace HotelCore.Application.Cleaning.Queries.GetTasksForStaff;

public class GetTasksForStaffQueryHandler(ICleaningTaskRepository cleaningTaskRepository)
    : IRequestHandler<GetTasksForStaffQuery, List<CleaningTaskDto>>
{
    public async Task<List<CleaningTaskDto>> Handle(GetTasksForStaffQuery query, CancellationToken cancellationToken)
    {
        var cleaningTasks = await cleaningTaskRepository.GetTasksForStaffAsync(query.StaffId, cancellationToken);

        return cleaningTasks
            .OrderBy(cleaningTask => cleaningTask.Priority)
            .Select(cleaningTask => new CleaningTaskDto(
                cleaningTask.Id,
                cleaningTask.Room?.RoomNumber ?? string.Empty,
                cleaningTask.RequestType.ToString(),
                cleaningTask.Status.ToString(),
                cleaningTask.ScheduledDate,
                cleaningTask.Priority,
                cleaningTask.AssignedStaffId,
                cleaningTask.AssignedStaff?.UserName))
            .ToList();
    }
}
