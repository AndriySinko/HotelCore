// This file contains code for CheckOutCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Reception.Commands.CheckOut;

public class CheckOutCommandValidator : AbstractValidator<CheckOutCommand>
{
    public CheckOutCommandValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
    }
}
