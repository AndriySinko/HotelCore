// This file contains code for SearchReservationsPublicQuery.
using MediatR;

namespace HotelCore.Application.Reception.Queries.SearchReservationsPublic;

public record SearchReservationsPublicQuery(string Query) : IRequest<List<PublicReservationDto>>;

public record PublicReservationDto(
    Guid Id,
    string QrCode,
    string GuestName,
    string GuestEmail,
    Guid RoomId,
    string RoomNumber,
    string RoomType,
    int NumberOfGuests,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    string Status,
    decimal TotalPrice);
