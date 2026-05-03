// This file contains code for CreateWorkRequestCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Commands.CreateWorkRequest;

public class CreateWorkRequestCommandValidator : AbstractValidator<CreateWorkRequestCommand>
{
    public CreateWorkRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.SeekerProfileId)
            .NotEmpty()
            .WithMessage("Seeker profile ID is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");

        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID is required");

        RuleFor(x => x.Budget)
            .GreaterThan(0)
            .When(x => x.Budget.HasValue)
            .WithMessage("Budget must be greater than zero");
    }
}
