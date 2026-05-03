// REST endpoints for the cleaning module
// access is split by role: guests can request, workers can complete, supervisors can assign and verify
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelCore.Api.Models;
using HotelCore.Application.Cleaning.Commands.AssignCleaningTask;
using HotelCore.Application.Cleaning.Commands.CancelCleaning;
using HotelCore.Application.Cleaning.Commands.CompleteCleaning;
using HotelCore.Application.Cleaning.Commands.RequestCleaning;
using HotelCore.Application.Cleaning.Commands.VerifyCleaning;
using HotelCore.Application.Cleaning.Queries.GetPendingCleaningTasks;
using HotelCore.Application.Cleaning.Queries.GetTasksForStaff;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CleaningController(IMediator mediator, ApplicationDbContext db) : ControllerBase
{
    // guests and receptionists submit a new cleaning request for a room
    [HttpPost("request")]
    [Authorize(Roles = "Guest,Receptionist,Administrator")]
    public async Task<IActionResult> RequestCleaning(
        [FromBody] RequestCleaningCommand command, CancellationToken ct)
    {
        var taskId = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(taskId));
    }

    // returns all tasks that are Requested or Assigned — used in the supervisor assignment panel
    [HttpGet("tasks/pending")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> GetPendingTasks(CancellationToken ct)
    {
        var tasks = await mediator.Send(new GetPendingCleaningTasksQuery(), ct);
        return Ok(ApiResult.Success(tasks));
    }

    // returns the task queue for a specific cleaning worker — ordered by priority
    [HttpGet("tasks/{staffId}")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Administrator")]
    public async Task<IActionResult> GetTasksForStaff(Guid staffId, CancellationToken ct)
    {
        var tasks = await mediator.Send(new GetTasksForStaffQuery(staffId), ct);
        return Ok(ApiResult.Success(tasks));
    }

    // supervisor assigns a pending task to a specific cleaning worker
    [HttpPost("tasks/{taskId}/assign")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignTaskRequest request, CancellationToken ct)
    {
        await mediator.Send(new AssignCleaningTaskCommand(taskId, request.StaffId), ct);
        return Ok(ApiResult.Success(true));
    }

    // cleaning worker marks a task as done after finishing the room
    [HttpPost("tasks/{taskId}/complete")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Administrator")]
    public async Task<IActionResult> Complete(Guid taskId, CancellationToken ct)
    {
        await mediator.Send(new CompleteCleaningCommand(taskId), ct);
        return Ok(ApiResult.Success(true));
    }

    // supervisor inspects the room and verifies the cleaning was done properly
    // only supervisors can verify — worker self-verification is not allowed
    [HttpPost("tasks/{taskId}/verify")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> Verify(Guid taskId, CancellationToken ct)
    {
        await mediator.Send(new VerifyCleaningCommand(taskId), ct);
        return Ok(ApiResult.Success(true));
    }

    // any involved party can cancel — a reason is required for the audit trail
    [HttpPost("tasks/{taskId}/cancel")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Receptionist,Administrator")]
    public async Task<IActionResult> Cancel(Guid taskId, [FromBody] string reason, CancellationToken ct)
    {
        await mediator.Send(new CancelCleaningCommand(taskId, reason), ct);
        return Ok(ApiResult.Success(true));
    }

    // returns all users with the CleaningWorker role — used to populate the worker dropdown in the assignment panel
    [HttpGet("workers")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> GetCleaningWorkers(CancellationToken ct)
    {
        var workers = await db.Users
            .Where(u => u.Role == UserRole.CleaningWorker && !u.IsDeleted)
            .Select(u => new { u.Id, Name = u.UserName ?? u.Email ?? string.Empty, u.Email })
            .ToListAsync(ct);
        return Ok(ApiResult.Success(workers));
    }
}

public record AssignTaskRequest(Guid StaffId);
