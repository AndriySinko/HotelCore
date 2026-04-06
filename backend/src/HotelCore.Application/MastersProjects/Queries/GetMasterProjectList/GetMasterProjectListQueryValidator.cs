using FluentValidation;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectList;

public class GetMasterProjectListQueryValidator : AbstractValidator<GetMasterProjectListQuery>
{
    public GetMasterProjectListQueryValidator()
    {
        RuleFor(x => x.Filter.WorkerProfileId)
            .NotEmpty()
            .When(x => x.Filter.WorkerProfileId.HasValue)
            .WithMessage("Worker profile ID is required");

        RuleFor(x => x.Filter.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Filter.Search))
            .WithMessage("Search must not exceed 200 characters");
    }
}
