// This file contains code for UnhideMasterProjectCommandValidator.
using FluentValidation;

namespace HotelCore.Application.MastersProjects.Commands.UnhideMasterProject;

public class UnhideMasterProjectCommandValidator : AbstractValidator<UnhideMasterProjectCommand>
{
    public UnhideMasterProjectCommandValidator()
    {
        RuleFor(x => x.MasterProjectId)
            .NotEmpty()
            .WithMessage("Master project ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
