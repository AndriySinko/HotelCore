// This file contains code for UnhideOrderCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Orders.Commands.UnhideOrder;

public class UnhideOrderCommandValidator : AbstractValidator<UnhideOrderCommand>
{
    public UnhideOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
