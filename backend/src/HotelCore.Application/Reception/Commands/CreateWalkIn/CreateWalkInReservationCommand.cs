// This file contains code for CreateWalkInReservationCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Reception.Commands.CreateWalkIn;






public record CreateWalkInReservationCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid RoomId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfGuests
) : IRequest<Guid>;
