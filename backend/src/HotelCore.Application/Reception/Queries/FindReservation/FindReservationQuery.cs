// This file contains code for FindReservationQuery.
using MediatR;

namespace HotelCore.Application.Reception.Queries.FindReservation;





public record FindReservationQuery(string? GuestName, string? QrCode) : IRequest<List<ReservationSummaryDto>>;

public record ReservationSummaryDto(
    Guid ReservationId,
    string GuestName,
    string GuestEmail,
    string RoomNumber,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    string Status);
