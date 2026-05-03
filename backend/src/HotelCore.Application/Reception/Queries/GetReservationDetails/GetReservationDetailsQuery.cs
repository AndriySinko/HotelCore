// This file contains code for GetReservationDetailsQuery.
using MediatR;

namespace HotelCore.Application.Reception.Queries.GetReservationDetails;

public record GetReservationDetailsQuery(Guid Id) : IRequest<ReservationDetailsDto?>;

public record ReservationDetailsDto(
    Guid Id,
    string ConfirmationNumber,
    GuestSummary Guest,
    RoomSummary Room,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int Nights,
    string Status,
    decimal TotalAmount
);

public record GuestSummary(Guid Id, string FirstName, string LastName, string Email, string? Phone);
public record RoomSummary(Guid Id, string Number, int Floor, RoomTypeSummary RoomType, string Status);
public record RoomTypeSummary(string Name, decimal PricePerNight);
