// This file contains code for CreateReservationCommand.
using MediatR;

namespace HotelCore.Application.Reception.Commands.CreateReservation;

public record CreateReservationCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid RoomId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfGuests
) : IRequest<CreateReservationResultDto>;

public record CreateReservationResultDto(
    Guid ReservationId,
    string QrCode,
    string RoomNumber,
    decimal TotalPrice);
