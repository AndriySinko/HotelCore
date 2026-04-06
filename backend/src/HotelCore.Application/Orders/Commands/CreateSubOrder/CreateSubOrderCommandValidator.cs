using FluentValidation;

namespace HotelCore.Application.Orders.Commands.CreateSubOrder;

public class CreateSubOrderCommandValidator : AbstractValidator<CreateSubOrderCommand>
{
    public CreateSubOrderCommandValidator()
    {
        RuleFor(x => x.ParentOrderId)
            .NotEmpty()
            .WithMessage("Parent order ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500)
            .WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description is not null)
            .WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Price must be greater than zero");

        RuleFor(x => x.PaymentType)
            .IsInEnum()
            .When(x => x.PaymentType.HasValue)
            .WithMessage("Invalid payment type");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
