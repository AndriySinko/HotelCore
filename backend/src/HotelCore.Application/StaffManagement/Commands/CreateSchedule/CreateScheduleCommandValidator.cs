// This file contains code for CreateScheduleCommandValidator.
using FluentValidation;

namespace HotelCore.Application.StaffManagement.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.PeriodStart).NotEmpty();
        RuleFor(x => x.PeriodEnd)
            .NotEmpty()
            .GreaterThan(x => x.PeriodStart)
            .WithMessage("End date must be after start date");
        RuleFor(x => x.CreatedByUserId).NotEmpty();
    }
}
