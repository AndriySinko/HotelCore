using FluentValidation;

namespace HotelCore.Application.Orders.Commands.HideOrder;

public class HideOrderCommandValidator : AbstractValidator<HideOrderCommand>
{
    public HideOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
