using System.Net.Http.Json;
using HotelCore.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelCore.Infrastructure.Services;

public class ResendEmailService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ResendEmailService> logger) : IEmailService
{
    public async Task SendReservationConfirmationAsync(
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
        var fromEmail = configuration["Resend:FromEmail"] ?? "HotelCore HMS <onboarding@resend.dev>";
        var nights = (checkOut - checkIn).Days;

        var payload = new
        {
            from = fromEmail,
            to = new[] { toEmail },
            subject = $"Reservation Confirmed - {qrCode}",
            html = BuildHtml(guestName, qrCode, roomNumber, numberOfGuests, checkIn, checkOut, nights, totalPrice, reservationUrl)
        };

        try
        {
            var client = httpClientFactory.CreateClient("Resend");
            var response = await client.PostAsJsonAsync("emails", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("[Resend] Failed to send to {Email}: {Status} - {Body}", toEmail, response.StatusCode, body);
            }
            else
            {
                logger.LogInformation("[Resend] Confirmation sent to {Email} for reservation {Code}", toEmail, qrCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Resend] Exception sending to {Email}", toEmail);
        }
    }

    private static string BuildHtml(
        string guestName, string qrCode, string roomNumber, int numberOfGuests,
        DateTime checkIn, DateTime checkOut, int nights, decimal totalPrice, string reservationUrl)
    {
        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
        <body style="margin:0;padding:0;background:#f3f4f6;font-family:system-ui,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f3f4f6;padding:40px 16px;">
            <tr><td align="center">
              <table width="560" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 1px 4px rgba(0,0,0,.08);">

                <!-- Header -->
                <tr><td style="background:#1a56db;padding:28px 32px;">
                  <p style="margin:0;font-size:22px;font-weight:700;color:#ffffff;">🏨 HotelCore HMS</p>
                  <p style="margin:6px 0 0;font-size:14px;color:#bfdbfe;">Hotel Management System</p>
                </td></tr>

                <!-- Body -->
                <tr><td style="padding:32px;">
                  <p style="margin:0 0 8px;font-size:18px;font-weight:600;color:#111827;">Hi {guestName},</p>
                  <p style="margin:0 0 24px;font-size:15px;color:#4b5563;">Your reservation is confirmed. Present the code below at the reception desk on arrival.</p>

                  <!-- Confirmation Code -->
                  <div style="background:#eff6ff;border:2px solid #bfdbfe;border-radius:8px;padding:20px;text-align:center;margin-bottom:24px;">
                    <p style="margin:0 0 4px;font-size:12px;font-weight:600;color:#1d4ed8;text-transform:uppercase;letter-spacing:.08em;">Confirmation Code</p>
                    <p style="margin:0;font-size:32px;font-weight:700;color:#1a56db;letter-spacing:.12em;">{qrCode}</p>
                  </div>

                  <!-- Details -->
                  <table width="100%" cellpadding="0" cellspacing="0" style="border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;margin-bottom:24px;">
                    <tr style="background:#f9fafb;">
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;width:45%;">Room</td>
                      <td style="padding:10px 16px;font-size:14px;font-weight:600;color:#111827;">{roomNumber}</td>
                    </tr>
                    <tr>
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;border-top:1px solid #e5e7eb;">Guests</td>
                      <td style="padding:10px 16px;font-size:14px;color:#111827;border-top:1px solid #e5e7eb;">{numberOfGuests}</td>
                    </tr>
                    <tr style="background:#f9fafb;">
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;border-top:1px solid #e5e7eb;">Check-in</td>
                      <td style="padding:10px 16px;font-size:14px;color:#111827;border-top:1px solid #e5e7eb;">{checkIn:dddd, MMMM d, yyyy} at 14:00</td>
                    </tr>
                    <tr>
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;border-top:1px solid #e5e7eb;">Check-out</td>
                      <td style="padding:10px 16px;font-size:14px;color:#111827;border-top:1px solid #e5e7eb;">{checkOut:dddd, MMMM d, yyyy} at 11:00</td>
                    </tr>
                    <tr style="background:#f9fafb;">
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;border-top:1px solid #e5e7eb;">Duration</td>
                      <td style="padding:10px 16px;font-size:14px;color:#111827;border-top:1px solid #e5e7eb;">{nights} night{(nights == 1 ? "" : "s")}</td>
                    </tr>
                    <tr>
                      <td style="padding:10px 16px;font-size:13px;font-weight:600;color:#6b7280;border-top:1px solid #e5e7eb;">Total</td>
                      <td style="padding:10px 16px;font-size:16px;font-weight:700;color:#1a56db;border-top:1px solid #e5e7eb;">${totalPrice:F2}</td>
                    </tr>
                  </table>

                  <!-- CTA Button -->
                  <div style="text-align:center;margin-bottom:24px;">
                    <a href="{reservationUrl}" style="display:inline-block;background:#1a56db;color:#ffffff;font-size:15px;font-weight:600;text-decoration:none;padding:12px 28px;border-radius:8px;">
                      View Reservation Details
                    </a>
                  </div>

                  <p style="margin:0;font-size:13px;color:#9ca3af;text-align:center;">
                    Or copy this link: <a href="{reservationUrl}" style="color:#1a56db;">{reservationUrl}</a>
                  </p>
                </td></tr>

                <!-- Footer -->
                <tr><td style="padding:16px 32px;background:#f9fafb;border-top:1px solid #e5e7eb;">
                  <p style="margin:0;font-size:12px;color:#9ca3af;text-align:center;">HotelCore HMS · This is an automated message, please do not reply.</p>
                </td></tr>

              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
    }
}
