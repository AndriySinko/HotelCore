// This file contains code for FindReservationQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Reception.Queries.FindReservation;






public class FindReservationQueryHandler(IReservationRepository reservationRepo)
    : IRequestHandler<FindReservationQuery, List<ReservationSummaryDto>>
{
    public async Task<List<ReservationSummaryDto>> Handle(FindReservationQuery query, CancellationToken ct)
    {
        var term = query.QrCode?.Trim() ?? query.GuestName?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(term))
            throw new BadRequestException("Provide either GuestName or QrCode to search");

        var results = await reservationRepo.SearchPublicAsync(term, ct);

        return results.Select(x => new ReservationSummaryDto(
            x.Reservation.Id,
            x.GuestName,
            x.GuestEmail,
            x.Reservation.Room?.RoomNumber ?? string.Empty,
            x.Reservation.CheckInDate,
            x.Reservation.CheckOutDate,
            x.Reservation.Status.ToString()
        )).ToList();
    }
}
