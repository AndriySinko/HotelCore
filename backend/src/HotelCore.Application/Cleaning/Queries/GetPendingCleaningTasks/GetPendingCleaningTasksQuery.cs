// This file contains code for GetPendingCleaningTasksQuery.
using MediatR;
using HotelCore.Application.Cleaning.Queries.GetTasksForStaff;

namespace HotelCore.Application.Cleaning.Queries.GetPendingCleaningTasks;

public record GetPendingCleaningTasksQuery : IRequest<List<CleaningTaskDto>>;
