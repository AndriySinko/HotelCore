// This file contains code for CreateWalkInReservationCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Reception.Commands.CreateWalkIn;








public class CreateWalkInReservationCommandHandler(
    IRoomRepository roomRepo,
    IReservationRepository reservationRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateWalkInReservationCommand, Guid>
{
    public async Task<Guid> Handle(CreateWalkInReservationCommand command, CancellationToken ct)
    {
        var room = await roomRepo.GetByIdAsync(command.RoomId, ct)
            ?? throw new NotFoundException(nameof(Room), command.RoomId);

        if (!room.IsAvailable())
            throw new ConflictException($"Room {room.RoomNumber} is not available for walk-in booking");

        var guest = new Guest
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            UserName = command.Email,
            Role = UserRole.Guest
        };

        var reservation = new Reservation
        {
            GuestId = guest.Id,
            Guest = guest,
            RoomId = command.RoomId,
            CheckInDate = command.CheckInDate,
            CheckOutDate = command.CheckOutDate,
            NumberOfGuests = command.NumberOfGuests,
            IsWalkIn = true,
            
            
            QrCode = $"WALKIN-{guest.Id}"
        };
        reservation.SetStatus(ReservationStatus.Reserved);

        
        
        
        var pendingPayment = new Payment
        {
            ReservationId = reservation.Id,
            Amount = room.PricePerNight * (decimal)(command.CheckOutDate - command.CheckInDate).TotalDays,
            Method = PaymentMethod.Cash,
            TransactionDate = DateTime.UtcNow
        };
        pendingPayment.SetStatus(PaymentStatus.Pending);
        reservation.Payments.Add(pendingPayment);

        room.SetStatus(RoomStatus.Reserved);

        await reservationRepo.AddAsync(reservation, ct);
        roomRepo.Update(room);
        await unitOfWork.SaveChangesAsync(ct);

        return reservation.Id;
    }
}
