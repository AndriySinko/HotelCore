// This file contains code for IEmailService.
namespace HotelCore.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendReservationConfirmationAsync(
        string toEmail,
        string guestName,
        Guid reservationId,
        string qrCode,
        string roomNumber,
        int numberOfGuests,
        DateTime checkIn,
        DateTime checkOut,
        decimal totalPrice,
        string reservationUrl,
        CancellationToken ct = default);
}
