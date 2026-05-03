// endpoints for the staff scheduling module
// managers create and publish schedules, staff see their own shifts
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Application.StaffManagement.Commands.AssignShift;
using HotelCore.Application.StaffManagement.Commands.CreateSchedule;
using HotelCore.Application.StaffManagement.Commands.PublishSchedule;
using HotelCore.Application.StaffManagement.Commands.SaveDraft;
using HotelCore.Application.StaffManagement.Queries.GetEmployees;
using HotelCore.Application.StaffManagement.Queries.GetSchedule;
using HotelCore.Application.StaffManagement.Queries.GetMyShifts;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController(IMediator mediator, IShiftRepository shiftRepo, IWorkScheduleRepository scheduleRepo, IUnitOfWork unitOfWork) : ControllerBase
{
    // returns all schedules with their shifts — used by the manager in the schedule list view
    // the response is mapped inline to avoid creating a separate DTO for this shape
    [HttpGet]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> GetAllSchedules(CancellationToken ct)
    {
        var schedules = await scheduleRepo.GetAllWithShiftsAsync(ct);
        var result = schedules.Select(s => new
        {
            s.Id,
            s.PeriodStart,
            s.PeriodEnd,
            Status = s.Status.ToString(),
            ShiftCount = s.Shifts.Count,
            Shifts = s.Shifts.Select(sh => new
            {
                sh.Id,
                sh.Date,
                sh.StartTime,
                sh.EndTime,
                ShiftType = sh.ShiftType.ToString(),
                sh.RequiredRole,
                Status = sh.Status.ToString(),
                sh.StaffMemberId,
                AssignedEmployeeName = sh.AssignedEmployee?.UserName,
            }).ToList(),
        }).ToList();
        return Ok(ApiResult.Success(result));
    }

    // creates a new schedule for a week period — starts in Draft status
    [HttpPost]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(id));
    }

    // removes all shifts from a schedule — used when the manager wants to rebuild the grid from scratch
    [HttpDelete("{scheduleId}/shifts")]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> ClearShifts(Guid scheduleId, CancellationToken ct)
    {
        await shiftRepo.DeleteByScheduleAsync(scheduleId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Ok(ApiResult.Success(true));
    }

    // adds a single shift to an existing schedule — validates overlap and contract hours
    [HttpPost("{scheduleId}/shifts")]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> AssignShift(Guid scheduleId, [FromBody] AssignShiftCommand command, CancellationToken ct)
    {
        var shiftId = await mediator.Send(command with { ScheduleId = scheduleId }, ct);
        return Ok(ApiResult.Success(shiftId));
    }

    // saves the current grid as a draft — does not notify staff
    [HttpPost("{scheduleId}/save-draft")]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> SaveDraft(Guid scheduleId, CancellationToken ct)
    {
        await mediator.Send(new SaveDraftCommand(scheduleId), ct);
        return Ok(ApiResult.Success(true));
    }

    // publishes the schedule — makes it visible to staff and sends notifications
    [HttpPost("{scheduleId}/publish")]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> Publish(Guid scheduleId, CancellationToken ct)
    {
        await mediator.Send(new PublishScheduleCommand(scheduleId), ct);
        return Ok(ApiResult.Success(true));
    }

    // returns a single schedule with all its shifts — used when the manager opens a specific week
    [HttpGet("{scheduleId}")]
    [Authorize(Roles = "HotelManager,Administrator")]
    public async Task<IActionResult> GetSchedule(Guid scheduleId, CancellationToken ct)
    {
        var schedule = await mediator.Send(new GetScheduleQuery(scheduleId), ct);
        return Ok(ApiResult.Success(schedule));
    }

    // staff members call this to see their own shifts from the latest published schedule
    [HttpGet("my-shifts/{staffId}")]
    [Authorize(Roles = "CleaningWorker,KitchenStaff,Receptionist,HotelManager,Supervisor,Administrator")]
    public async Task<IActionResult> GetMyShifts(Guid staffId, CancellationToken ct)
    {
        var schedule = await mediator.Send(new GetMyShiftsQuery(staffId), ct);
        return Ok(ApiResult.Success(schedule));
    }

    // returns employee list for shift assignment — managers can filter by department
    [HttpGet("employees")]
    [Authorize(Roles = "HotelManager,Administrator,CleaningWorker,KitchenStaff,Receptionist")]
    public async Task<IActionResult> GetEmployees([FromQuery] string? department, CancellationToken ct)
    {
        var employees = await mediator.Send(new GetEmployeesQuery(department), ct);
        return Ok(ApiResult.Success(employees));
    }
}
