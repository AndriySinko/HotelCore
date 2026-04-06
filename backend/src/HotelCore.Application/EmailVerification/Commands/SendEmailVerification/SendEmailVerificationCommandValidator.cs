using FluentValidation;

namespace HotelCore.Application.EmailVerification.Commands.SendEmailVerification;

public sealed class SendEmailVerificationCommandValidator : AbstractValidator<SendEmailVerificationCommand>
{
    public SendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
