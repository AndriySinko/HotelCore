using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Reception.Commands.CheckIn;










public class CheckInCommandHandler(
    IReservationRepository reservationRepo,
    IRoomRepository roomRepo,
    IGuestRepository guestRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<CheckInCommand, CheckInResultDto>
{
    public async Task<CheckInResultDto> Handle(CheckInCommand command, CancellationToken ct)
    {
        var reservation = await reservationRepo.GetByIdWithDetailsAsync(command.ReservationId, ct)
            ?? throw new NotFoundException(nameof(Reservation), command.ReservationId);

        // identity check is optional - walk-in guests may not have a profile yet
        var guest = await guestRepo.GetByIdNoTrackingAsync(reservation.GuestId, ct);
        if (guest != null)
        {
            guest.VerifyIdentity(command.IdType, command.IdNumber, command.IdExpiry);
            if (!guest.IsIdentityVerified)
                throw new BadRequestException("Identity verification failed");
        }

        var room = await ResolveRoomAsync(reservation, command, ct);

        var payment = CreatePayment(reservation, command.PaymentMethod);
        await reservationRepo.AddPaymentAsync(payment, ct);

        await unitOfWork.SaveChangesAsync(ct);

        await reservationRepo.SetStatusAsync(reservation.Id, ReservationStatus.CheckedIn, ct);
        await roomRepo.SetStatusAsync(room.Id, RoomStatus.Occupied, ct);

        var keyNumber = $"K-{room.RoomNumber}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        return new CheckInResultDto(true, keyNumber, room.RoomNumber);
    }

    // tries the reserved room first, falls back to alternatives if it is not ready
    private async Task<Room> ResolveRoomAsync(Reservation reservation, CheckInCommand command, CancellationToken ct)
    {
        var room = reservation.Room
            ?? await roomRepo.GetByIdAsync(reservation.RoomId, ct)
            ?? throw new NotFoundException(nameof(Room), reservation.RoomId);

        if (!room.IsAvailable() && room.Status != RoomStatus.Reserved)
        {
            // room was taken by another reservation or still under cleaning
            var alternatives = await roomRepo.GetAlternativeRoomsAsync(room.RoomType, reservation.CheckInDate, ct);

            if (alternatives.Count == 0)
                throw new ConflictException("No rooms available - original room is not ready and no alternatives exist");

            if (command.AlternativeRoomId is null)
                throw new ConflictException("Assigned room is not ready. Please select an alternative room.");

            room = alternatives.FirstOrDefault(r => r.Id == command.AlternativeRoomId)
                ?? throw new NotFoundException(nameof(Room), command.AlternativeRoomId);

            reservation.RoomId = room.Id;
        }

        return room;
    }

    private static Payment CreatePayment(Reservation reservation, PaymentMethod method)
    {
        // if dates are somehow same day, charge one nights worth as a minimum
        var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
        var amount = nights > 0 ? reservation.GetTotalCharges() : reservation.Room?.PricePerNight ?? 0m;

        var payment = new Payment
        {
            ReservationId   = reservation.Id,
            Amount          = amount,
            Method          = method,
            TransactionDate = DateTime.UtcNow,
            ReferenceNumber = $"REF-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };
        payment.SetStatus(PaymentStatus.Completed);
        return payment;
    }
}
