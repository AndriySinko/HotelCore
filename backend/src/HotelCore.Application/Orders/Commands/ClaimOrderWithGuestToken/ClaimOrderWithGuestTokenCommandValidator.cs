using FluentValidation;

namespace HotelCore.Application.Orders.Commands.ClaimOrderWithGuestToken;

public class ClaimOrderWithGuestTokenCommandValidator : AbstractValidator<ClaimOrderWithGuestTokenCommand>
{
    public ClaimOrderWithGuestTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("Access token is required")
            .MinimumLength(20)
            .WithMessage("Invalid access token format");

        RuleFor(x => x.GuestAccessToken)
            .NotEmpty()
            .WithMessage("Guest access token is required")
            .MinimumLength(10)
            .WithMessage("Guest access token must be at least 10 characters");
    }
}
