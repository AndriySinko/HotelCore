// This file contains code for GetActiveReservationsQuery.
using MediatR;

namespace HotelCore.Application.Reception.Queries.GetActiveReservations;

public record GetActiveReservationsQuery : IRequest<List<ActiveReservationDto>>;

public record ActiveReservationDto(
    Guid Id,
    string GuestName,
    string GuestEmail,
    string RoomNumber,
    string RoomType,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    string Status,
    string QrCode);
