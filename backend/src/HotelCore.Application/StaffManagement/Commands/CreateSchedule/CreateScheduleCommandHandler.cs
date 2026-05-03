// This file contains code for CreateScheduleCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.StaffManagement.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(
    IWorkScheduleRepository scheduleRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateScheduleCommand command, CancellationToken ct)
    {
        
        var existing = await scheduleRepo.GetByPeriodAsync(command.PeriodStart, command.PeriodEnd, ct);
        if (existing is not null)
            throw new ConflictException("A schedule already exists for this period");

        var schedule = new WorkSchedule
        {
            PeriodStart = command.PeriodStart,
            PeriodEnd = command.PeriodEnd,
            CreatedByUserId = command.CreatedByUserId
        };

        await scheduleRepo.AddAsync(schedule, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return schedule.Id;
    }
}
