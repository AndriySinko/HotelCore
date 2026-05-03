// This file contains code for CreateReservationCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Reception.Commands.CreateReservation;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.CheckInDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
        RuleFor(x => x.CheckOutDate).GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out must be after check-in");
        RuleFor(x => x.NumberOfGuests).InclusiveBetween(1, 10);
    }
}
