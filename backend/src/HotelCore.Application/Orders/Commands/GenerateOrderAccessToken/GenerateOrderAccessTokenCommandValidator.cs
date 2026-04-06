using FluentValidation;

namespace HotelCore.Application.Orders.Commands.GenerateOrderAccessToken;

public class GenerateOrderAccessTokenCommandValidator : AbstractValidator<GenerateOrderAccessTokenCommand>
{
    public GenerateOrderAccessTokenCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
