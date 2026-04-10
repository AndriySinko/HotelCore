// This file contains code for UpdateWorkRequestStatusCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequestStatus;

public class UpdateWorkRequestStatusCommandValidator : AbstractValidator<UpdateWorkRequestStatusCommand>
{
    public UpdateWorkRequestStatusCommandValidator()
    {
        RuleFor(x => x.WorkRequestId)
            .NotEmpty()
            .WithMessage("Work request ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid work request status");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");
    }
}
