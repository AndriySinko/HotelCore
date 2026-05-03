// This file contains code for DeleteMasterProjectCommandValidator.
using FluentValidation;

namespace HotelCore.Application.MastersProjects.Commands.DeleteMasterProject;

public class DeleteMasterProjectCommandValidator : AbstractValidator<DeleteMasterProjectCommand>
{
    public DeleteMasterProjectCommandValidator()
    {
        RuleFor(x => x.MasterProjectId)
            .NotEmpty()
            .WithMessage("Master project ID is required");
    }
}
