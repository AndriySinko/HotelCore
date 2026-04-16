// This file contains code for CleaningController.
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
    [HttpPost("request")]
    [Authorize(Roles = "Guest,Receptionist,Administrator")]
    public async Task<IActionResult> RequestCleaning(
        [FromBody] RequestCleaningCommand command, CancellationToken ct)
    {
        var taskId = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(taskId));
    }

    [HttpGet("tasks/pending")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> GetPendingTasks(CancellationToken ct)
    {
        var tasks = await mediator.Send(new GetPendingCleaningTasksQuery(), ct);
        return Ok(ApiResult.Success(tasks));
    }

    [HttpGet("tasks/{staffId}")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Administrator")]
    public async Task<IActionResult> GetTasksForStaff(Guid staffId, CancellationToken ct)
    {
        var tasks = await mediator.Send(new GetTasksForStaffQuery(staffId), ct);
        return Ok(ApiResult.Success(tasks));
    }

    [HttpPost("tasks/{taskId}/assign")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignTaskRequest request, CancellationToken ct)
    {
        await mediator.Send(new AssignCleaningTaskCommand(taskId, request.StaffId), ct);
        return Ok(ApiResult.Success(true));
    }

    [HttpPost("tasks/{taskId}/complete")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Administrator")]
    public async Task<IActionResult> Complete(Guid taskId, CancellationToken ct)
    {
        await mediator.Send(new CompleteCleaningCommand(taskId), ct);
        return Ok(ApiResult.Success(true));
    }

    [HttpPost("tasks/{taskId}/verify")]
    [Authorize(Roles = "Supervisor,Administrator")]
    public async Task<IActionResult> Verify(Guid taskId, CancellationToken ct)
    {
        await mediator.Send(new VerifyCleaningCommand(taskId), ct);
        return Ok(ApiResult.Success(true));
    }

    [HttpPost("tasks/{taskId}/cancel")]
    [Authorize(Roles = "CleaningWorker,Supervisor,Receptionist,Administrator")]
    public async Task<IActionResult> Cancel(Guid taskId, [FromBody] string reason, CancellationToken ct)
    {
        await mediator.Send(new CancelCleaningCommand(taskId, reason), ct);
        return Ok(ApiResult.Success(true));
    }

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
