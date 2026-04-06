using FluentValidation;

namespace HotelCore.Application.EmailVerification.Commands.VerifyEmailVerification;

public sealed class VerifyEmailVerificationCommandValidator : AbstractValidator<VerifyEmailVerificationCommand>
{
    public VerifyEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x)
            .Must(HaveCodeOrToken)
            .WithMessage("Either code or token must be provided.");
    }

    private static bool HaveCodeOrToken(VerifyEmailVerificationCommand command) =>
        !string.IsNullOrWhiteSpace(command.Code)
        || !string.IsNullOrWhiteSpace(command.Token);
}
