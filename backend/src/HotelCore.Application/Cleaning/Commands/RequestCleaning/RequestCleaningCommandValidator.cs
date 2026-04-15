// This file contains code for RequestCleaningCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Cleaning.Commands.RequestCleaning;

public class RequestCleaningCommandValidator : AbstractValidator<RequestCleaningCommand>
{
    public RequestCleaningCommandValidator()
    {
        RuleFor(command => command.RoomId).NotEmpty();
        RuleFor(command => command.RequestType).IsInEnum();
        RuleFor(command => command.ScheduledDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Scheduled date cannot be in the past");
        RuleFor(command => command.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage("Priority must be between 1 (highest) and 5 (lowest)");
    }
}
