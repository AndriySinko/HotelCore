// This file contains code for HideMasterProjectCommandValidator.
using FluentValidation;

namespace HotelCore.Application.MastersProjects.Commands.HideMasterProject;

public class HideMasterProjectCommandValidator : AbstractValidator<HideMasterProjectCommand>
{
    public HideMasterProjectCommandValidator()
    {
        RuleFor(x => x.MasterProjectId)
            .NotEmpty()
            .WithMessage("Master project ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
