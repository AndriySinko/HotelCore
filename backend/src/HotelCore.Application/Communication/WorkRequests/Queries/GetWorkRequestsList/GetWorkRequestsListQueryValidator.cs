// This file contains code for GetWorkRequestsListQueryValidator.
using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestsList;

public class GetWorkRequestsListQueryValidator : AbstractValidator<GetWorkRequestsListQuery>
{
    public GetWorkRequestsListQueryValidator()
    {
        RuleFor(x => x.Filter.Status)
            .IsInEnum()
            .When(x => x.Filter.Status.HasValue)
            .WithMessage("Invalid work request status");

        RuleFor(x => x.Filter.ExcludeStatus)
            .IsInEnum()
            .When(x => x.Filter.ExcludeStatus.HasValue)
            .WithMessage("Invalid work request status");

        RuleFor(x => x.Filter.MinBudget)
            .GreaterThan(0)
            .When(x => x.Filter.MinBudget.HasValue)
            .WithMessage("Minimum budget must be greater than zero");

        RuleFor(x => x.Filter.MaxBudget)
            .GreaterThan(0)
            .When(x => x.Filter.MaxBudget.HasValue)
            .WithMessage("Maximum budget must be greater than zero");

        RuleFor(x => x.Filter)
            .Must(filter => !(filter.MinBudget.HasValue && filter.MaxBudget.HasValue) || filter.MinBudget <= filter.MaxBudget)
            .WithMessage("Minimum budget must be less than or equal to maximum budget");
    }
}
