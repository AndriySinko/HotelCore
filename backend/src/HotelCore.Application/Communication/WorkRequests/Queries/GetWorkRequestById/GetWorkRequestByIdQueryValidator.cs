// This file contains code for GetWorkRequestByIdQueryValidator.
using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestById;

public class GetWorkRequestByIdQueryValidator : AbstractValidator<GetWorkRequestByIdQuery>
{
    public GetWorkRequestByIdQueryValidator()
    {
        RuleFor(x => x.WorkRequestId)
            .NotEmpty()
            .WithMessage("Work request ID is required");
    }
}
