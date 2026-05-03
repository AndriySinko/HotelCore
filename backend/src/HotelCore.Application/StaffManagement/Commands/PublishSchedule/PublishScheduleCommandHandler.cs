// This file contains code for PublishScheduleCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.StaffManagement.Commands.PublishSchedule;

public class PublishScheduleCommandHandler(
    IWorkScheduleRepository scheduleRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<PublishScheduleCommand>
{
    public async Task Handle(PublishScheduleCommand command, CancellationToken ct)
    {
        var schedule = await scheduleRepo.GetByIdWithShiftsAsync(command.ScheduleId, ct)
            ?? throw new NotFoundException(nameof(WorkSchedule), command.ScheduleId);

        if (schedule.Status == ScheduleStatus.Published)
            throw new ConflictException("Schedule is already published");

        
        var uncoveredShifts = schedule.Shifts.Where(s => s.Status == ShiftStatus.Uncovered).ToList();
        if (uncoveredShifts.Count > 0)
        {
            var dates = string.Join(", ", uncoveredShifts.Select(s => s.Date.ToString("yyyy-MM-dd")).Distinct());
            throw new BadRequestException(
                $"Cannot publish schedule — {uncoveredShifts.Count} uncovered shift(s) remain on: {dates}");
        }

        schedule.SetStatus(ScheduleStatus.Published);
        scheduleRepo.Update(schedule);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
