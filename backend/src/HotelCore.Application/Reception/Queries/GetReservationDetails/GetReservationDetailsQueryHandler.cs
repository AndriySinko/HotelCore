// This file contains code for GetReservationDetailsQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;

namespace HotelCore.Application.Reception.Queries.GetReservationDetails;

public class GetReservationDetailsQueryHandler(IReservationRepository reservationRepo)
    : IRequestHandler<GetReservationDetailsQuery, ReservationDetailsDto?>
{
    public async Task<ReservationDetailsDto?> Handle(GetReservationDetailsQuery query, CancellationToken ct)
    {
        var r = await reservationRepo.GetByIdWithDetailsAsync(query.Id, ct);
        if (r is null) return null;

        var room = r.Room;

        var (firstName, lastName, email, phone) = await reservationRepo.GetGuestInfoAsync(r.GuestId, ct);
        var guestSummary = new GuestSummary(r.GuestId, firstName, lastName, email, phone);

        var roomSummary = room is null
            ? new RoomSummary(r.RoomId, "?", 0, new RoomTypeSummary("Unknown", 0), "Unknown")
            : new RoomSummary(
                room.Id,
                room.RoomNumber,
                room.Floor,
                new RoomTypeSummary(room.RoomType.ToString(), room.PricePerNight),
                room.Status.ToString());

        var nights = Math.Max(1, (r.CheckOutDate - r.CheckInDate).Days);
        var total  = r.Payments.Sum(p => p.Amount);
        if (total == 0 && room is not null)
            total = nights * room.PricePerNight;

        return new ReservationDetailsDto(
            r.Id,
            r.QrCode,
            guestSummary,
            roomSummary,
            r.CheckInDate,
            r.CheckOutDate,
            nights,
            r.Status.ToString(),
            total
        );
    }
}
