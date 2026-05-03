using HotelCore.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HotelCore.Infrastructure.Services;

public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendReservationConfirmationAsync(
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
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL] To: {Email} | Subject: HotelCore Reservation Confirmed\n" +
            "  Guest: {Name}\n" +
            "  Confirmation: {QrCode}\n" +
            "  Room: {Room} | Guests: {Guests}\n" +
            "  Check-in: {CheckIn:yyyy-MM-dd}\n" +
            "  Check-out: {CheckOut:yyyy-MM-dd}\n" +
            "  Total: ${Total:F2}\n" +
            "  Details: {Url}",
            toEmail, guestName, qrCode, roomNumber, numberOfGuests,
            checkIn, checkOut, totalPrice, reservationUrl);

        return Task.CompletedTask;
    }
}
