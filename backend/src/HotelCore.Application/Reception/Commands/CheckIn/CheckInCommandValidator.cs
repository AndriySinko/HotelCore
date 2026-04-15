// This file contains code for CheckInCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Reception.Commands.CheckIn;






public class CheckInCommandValidator : AbstractValidator<CheckInCommand>
{
    public CheckInCommandValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.IdType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IdNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IdExpiry)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Identity document is expired and cannot be used for check-in");
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}
