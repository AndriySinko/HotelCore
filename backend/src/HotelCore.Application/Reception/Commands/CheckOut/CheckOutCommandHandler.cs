// This file contains code for CheckOutCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Reception.Commands.CheckOut;








public class CheckOutCommandHandler(
    IReservationRepository reservationRepo,
    IRoomRepository roomRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<CheckOutCommand, CheckOutResultDto>
{
    public async Task<CheckOutResultDto> Handle(CheckOutCommand command, CancellationToken ct)
    {
        var reservation = await reservationRepo.GetByIdWithDetailsAsync(command.ReservationId, ct)
            ?? throw new NotFoundException(nameof(Reservation), command.ReservationId);

        if (reservation.Status != ReservationStatus.CheckedIn)
            throw new BadRequestException(
                $"Cannot check out a reservation that is in '{reservation.Status}' status");

        var room = reservation.Room
            ?? await roomRepo.GetByIdAsync(reservation.RoomId, ct)
            ?? throw new NotFoundException(nameof(Room), reservation.RoomId);

        await unitOfWork.SaveChangesAsync(ct);

        await reservationRepo.SetStatusAsync(reservation.Id, ReservationStatus.CheckedOut, ct);
        await roomRepo.SetStatusAsync(room.Id, RoomStatus.UnderCleaning, ct);

        var guestName = reservation.Guest?.GetFullName() ?? "Guest";
        var totalCharged = reservation.Payments.Sum(p => p.Amount);

        return new CheckOutResultDto(true, guestName, totalCharged, room.RoomNumber);
    }
}
