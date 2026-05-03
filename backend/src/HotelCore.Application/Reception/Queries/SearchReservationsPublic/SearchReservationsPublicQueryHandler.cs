// This file contains code for SearchReservationsPublicQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;

namespace HotelCore.Application.Reception.Queries.SearchReservationsPublic;

public class SearchReservationsPublicQueryHandler(IReservationRepository reservationRepo)
    : IRequestHandler<SearchReservationsPublicQuery, List<PublicReservationDto>>
{
    public async Task<List<PublicReservationDto>> Handle(SearchReservationsPublicQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
            return [];

        var results = await reservationRepo.SearchPublicAsync(request.Query, ct);

        return results.Select(x => new PublicReservationDto(
            x.Reservation.Id,
            x.Reservation.QrCode,
            x.GuestName,
            x.GuestEmail,
            x.Reservation.RoomId,
            x.Reservation.Room?.RoomNumber ?? "",
            x.Reservation.Room?.RoomType.ToString() ?? "",
            x.Reservation.NumberOfGuests,
            x.Reservation.CheckInDate,
            x.Reservation.CheckOutDate,
            x.Reservation.Status.ToString(),
            x.Reservation.GetTotalCharges()
        )).ToList();
    }
}
