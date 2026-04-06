using FluentValidation;

namespace HotelCore.Application.Orders.Commands.MoveGuestOrdersToAccount;

public class MoveGuestOrdersToAccountCommandValidator : AbstractValidator<MoveGuestOrdersToAccountCommand>
{
    public MoveGuestOrdersToAccountCommandValidator()
    {
        RuleFor(x => x.GuestAccessToken)
            .NotEmpty()
            .WithMessage("Guest access token is required")
            .MinimumLength(10)
            .WithMessage("Guest access token must be at least 10 characters");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
