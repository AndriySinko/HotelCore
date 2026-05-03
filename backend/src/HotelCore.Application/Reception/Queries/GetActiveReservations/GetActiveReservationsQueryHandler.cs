// This file contains code for GetActiveReservationsQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;

namespace HotelCore.Application.Reception.Queries.GetActiveReservations;

public class GetActiveReservationsQueryHandler(IReservationRepository reservationRepo)
    : IRequestHandler<GetActiveReservationsQuery, List<ActiveReservationDto>>
{
    public async Task<List<ActiveReservationDto>> Handle(GetActiveReservationsQuery query, CancellationToken ct)
    {
        var reservations = await reservationRepo.GetActiveReservationsWithNamesAsync(ct);

        return reservations.Select(x => new ActiveReservationDto(
            x.Reservation.Id,
            x.GuestName,
            x.GuestEmail,
            x.Reservation.Room?.RoomNumber ?? string.Empty,
            x.Reservation.Room?.RoomType.ToString() ?? string.Empty,
            x.Reservation.CheckInDate,
            x.Reservation.CheckOutDate,
            x.Reservation.Status.ToString(),
            x.Reservation.QrCode ?? string.Empty
        )).ToList();
    }
}
