// This file contains code for AssignShiftCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.Services;

namespace HotelCore.Application.StaffManagement.Commands.AssignShift;

public class AssignShiftCommandHandler(
    IWorkScheduleRepository scheduleRepo,
    IStaffRepository staffRepo,
    IShiftRepository shiftRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignShiftCommand, Guid>
{
    public async Task<Guid> Handle(AssignShiftCommand command, CancellationToken ct)
    {
        var schedule = await scheduleRepo.GetByIdAsync(command.ScheduleId, ct)
            ?? throw new NotFoundException(nameof(WorkSchedule), command.ScheduleId);

        var staff = await staffRepo.GetByIdAsync(command.StaffMemberId, ct)
            ?? throw new NotFoundException("StaffMember", command.StaffMemberId);

        
        var windowStart = command.Date.AddDays(-1);
        var windowEnd   = command.Date.AddDays(1);
        var existingShifts = await shiftRepo.GetByStaffAsync(
            command.StaffMemberId, windowStart, windowEnd, ct);

        if (ShiftConflictService.HasConflict(command.Date, command.StartTime, command.EndTime, existingShifts))
            throw new ConflictException($"{staff.Position} already has a shift that overlaps with {command.Date:yyyy-MM-dd} {command.StartTime}–{command.EndTime}");

        
        var weekStart = command.Date.AddDays(-(int)command.Date.DayOfWeek + (int)DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(7);
        var weeklyShifts = await shiftRepo.GetByStaffAsync(command.StaffMemberId, weekStart, weekEnd, ct);
        var currentWeeklyHours = weeklyShifts.Sum(s => s.GetDurationHours());
        var newShiftHours = (decimal)(command.EndTime - command.StartTime).TotalHours;
        if (currentWeeklyHours + newShiftHours > staff.ContractHoursPerWeek)
            throw new ConflictException(
                $"Assigning this shift would exceed {staff.Position}'s weekly contract limit of {staff.ContractHoursPerWeek}h " +
                $"(current: {currentWeeklyHours:F1}h, adding: {newShiftHours:F1}h)");

        var shift = new Shift
        {
            WorkScheduleId = command.ScheduleId,
            StaffMemberId = command.StaffMemberId,
            Date = command.Date,
            StartTime = command.StartTime,
            EndTime = command.EndTime,
            ShiftType = command.ShiftType,
            RequiredRole = command.RequiredRole
        };
        shift.SetStatus(ShiftStatus.Assigned);

        await shiftRepo.AddAsync(shift, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return shift.Id;
    }
}
