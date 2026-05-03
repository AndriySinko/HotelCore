// This file contains code for UpdateOrderCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");

        RuleFor(x => x.Title)
            .MaximumLength(500)
            .When(x => x.Title is not null)
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
    }
}
