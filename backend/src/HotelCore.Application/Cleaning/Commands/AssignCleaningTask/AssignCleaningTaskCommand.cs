// This file contains code for AssignCleaningTaskCommand.
using MediatR;

namespace HotelCore.Application.Cleaning.Commands.AssignCleaningTask;

public record AssignCleaningTaskCommand(Guid TaskId, Guid StaffId) : IRequest;
