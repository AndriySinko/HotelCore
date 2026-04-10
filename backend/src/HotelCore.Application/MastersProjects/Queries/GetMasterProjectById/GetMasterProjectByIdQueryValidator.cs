// This file contains code for GetMasterProjectByIdQueryValidator.
using FluentValidation;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectById;

public class GetMasterProjectByIdQueryValidator : AbstractValidator<GetMasterProjectByIdQuery>
{
    public GetMasterProjectByIdQueryValidator()
    {
        RuleFor(x => x.masterProjectId)
            .NotEmpty()
            .WithMessage("Master project ID is required");
    }
}
