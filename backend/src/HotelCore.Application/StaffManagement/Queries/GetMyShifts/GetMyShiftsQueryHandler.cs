// This file contains code for GetMyShiftsQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Application.StaffManagement.Queries.GetSchedule;

namespace HotelCore.Application.StaffManagement.Queries.GetMyShifts;

public class GetMyShiftsQueryHandler(IWorkScheduleRepository scheduleRepo)
    : IRequestHandler<GetMyShiftsQuery, ScheduleDto?>
{
    public async Task<ScheduleDto?> Handle(GetMyShiftsQuery query, CancellationToken ct)
    {
        var schedule = await scheduleRepo.GetLatestPublishedByStaffIdAsync(query.StaffId, ct);
        if (schedule is null) return null;

        return new ScheduleDto(
            schedule.Id,
            schedule.PeriodStart,
            schedule.PeriodEnd,
            schedule.Status.ToString(),
            schedule.Shifts.Select(s => new ShiftDto(
                s.Id,
                s.Date,
                s.StartTime,
                s.EndTime,
                s.ShiftType.ToString(),
                s.RequiredRole,
                s.Status.ToString(),
                s.StaffMemberId,
                s.AssignedEmployee != null ? s.AssignedEmployee.UserName : null
            )).ToList()
        );
    }
}