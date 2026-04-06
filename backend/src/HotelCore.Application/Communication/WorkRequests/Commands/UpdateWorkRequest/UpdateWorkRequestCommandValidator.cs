using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequest;

public class UpdateWorkRequestCommandValidator : AbstractValidator<UpdateWorkRequestCommand>
{
    public UpdateWorkRequestCommandValidator()
    {
        RuleFor(x => x.WorkRequestId)
            .NotEmpty()
            .WithMessage("Work request ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x => x.Title is not null)
            .WithMessage("Title must not be empty")
            .MaximumLength(200)
            .When(x => x.Title is not null)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .When(x => x.Description is not null)
            .WithMessage("Description must not be empty")
            .MaximumLength(2000)
            .When(x => x.Description is not null)
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Budget)
            .GreaterThan(0)
            .When(x => x.Budget.HasValue)
            .WithMessage("Budget must be greater than zero");
    }
}
