// This file contains code for CreateReservationCommandHandler.
using MediatR;
using Microsoft.Extensions.Configuration;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Reception.Commands.CreateReservation;

public class CreateReservationCommandHandler(
    IRoomRepository roomRepo,
    IReservationRepository reservationRepo,
    IGuestRepository guestRepo,
    IUnitOfWork unitOfWork,
    IQrCodeService qrCodeService,
    IEmailService emailService,
    IConfiguration configuration
) : IRequestHandler<CreateReservationCommand, CreateReservationResultDto>
{
    public async Task<CreateReservationResultDto> Handle(CreateReservationCommand command, CancellationToken ct)
    {
        var room = await roomRepo.GetByIdAsync(command.RoomId, ct)
            ?? throw new NotFoundException(nameof(Room), command.RoomId);

        if (!room.IsAvailable())
            throw new ConflictException($"Room {room.RoomNumber} is not available for the requested dates");

        var confirmationCode = GenerateConfirmationCode();

        
        var existingUserId = await guestRepo.FindUserIdByEmailAsync(command.Email, ct);

        Guest? newGuest = null;
        Guid guestId;

        if (existingUserId.HasValue)
        {
            
            guestId = existingUserId.Value;
        }
        else
        {
            newGuest = new Guest
            {
                FirstName          = command.FirstName,
                LastName           = command.LastName,
                Email              = command.Email,
                UserName           = command.Email,
                NormalizedEmail    = command.Email.ToUpperInvariant(),
                NormalizedUserName = command.Email.ToUpperInvariant(),
                Phone              = command.Phone,
                Role               = UserRole.Guest
            };
            guestId = newGuest.Id;
        }

        var reservation = new Reservation
        {
            GuestId = guestId,
            Guest = newGuest,
            RoomId = command.RoomId,
            Room = room,
            CheckInDate = command.CheckInDate,
            CheckOutDate = command.CheckOutDate,
            NumberOfGuests = command.NumberOfGuests,
            IsWalkIn = false,
            QrCode = confirmationCode
        };

        var frontendBaseUrl = configuration["FrontendBaseUrl"] ?? "http://localhost:5173";
        var reservationUrl = $"{frontendBaseUrl}/reservation/{confirmationCode}";

        var qrPath = await qrCodeService.GenerateAsync(reservationUrl, reservation.Id.ToString(), ct);

        room.SetStatus(RoomStatus.Reserved);

        await reservationRepo.AddAsync(reservation, ct);
        roomRepo.Update(room);
        await unitOfWork.SaveChangesAsync(ct);

        var totalPrice = reservation.GetTotalCharges();

        var guestName = newGuest?.GetFullName() ?? $"{command.FirstName} {command.LastName}";

        await emailService.SendReservationConfirmationAsync(
            command.Email,
            guestName,
            reservation.Id,
            confirmationCode,
            room.RoomNumber,
            command.NumberOfGuests,
            command.CheckInDate,
            command.CheckOutDate,
            totalPrice,
            reservationUrl,
            ct);

        return new CreateReservationResultDto(reservation.Id, confirmationCode, room.RoomNumber, totalPrice);
    }

    private static string GenerateConfirmationCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var suffix = new string(Enumerable.Range(0, 6)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
        return $"HC-{suffix}";
    }
}
