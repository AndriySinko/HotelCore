using FluentValidation;

namespace HotelCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
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

        RuleFor(x => x.ClientEmail)
            .EmailAddress()
            .When(x => x.ClientEmail is not null)
            .WithMessage("Invalid email address");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .WithMessage("Creator user ID is required");
    }
}
