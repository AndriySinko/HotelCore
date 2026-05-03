// This file contains code for DeleteWorkRequestCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Commands.DeleteWorkRequest;

public class DeleteWorkRequestCommandValidator : AbstractValidator<DeleteWorkRequestCommand>
{
    public DeleteWorkRequestCommandValidator()
    {
        RuleFor(x => x.WorkRequestId)
            .NotEmpty()
            .WithMessage("Work request ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
