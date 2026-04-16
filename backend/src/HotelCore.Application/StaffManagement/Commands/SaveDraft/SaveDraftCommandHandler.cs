// This file contains code for SaveDraftCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.StaffManagement.Commands.SaveDraft;

public class SaveDraftCommandHandler(
    IWorkScheduleRepository scheduleRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<SaveDraftCommand>
{
    public async Task Handle(SaveDraftCommand command, CancellationToken ct)
    {
        var schedule = await scheduleRepo.GetByIdAsync(command.ScheduleId, ct)
            ?? throw new NotFoundException(nameof(WorkSchedule), command.ScheduleId);

        
        if (schedule.Status != ScheduleStatus.Draft)
            throw new BadRequestException("Only draft schedules can be saved as draft");

        schedule.UpdatedAt = DateTime.UtcNow;
        scheduleRepo.Update(schedule);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
