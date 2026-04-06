using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Infrastructure.Notifications;

public sealed class HttpVerificationNotificationPublisher(
    HttpClient httpClient,
    ILogger<HttpVerificationNotificationPublisher> logger) : IVerificationNotificationPublisher
{
    public async Task PublishSendVerificationEmailAsync(SendVerificationEmailNotification notification, CancellationToken cancellationToken)
    {
        var request = new SendVerificationEmailRequest(
            notification.UserEmail,
            notification.UserId,
            notification.Code,
            notification.Link);

        using var response = await httpClient.PostAsJsonAsync("api/notification/sendVerificationEmail", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "NotificationService returned {StatusCode} for {Email}. Body: {Body}",
                (int)response.StatusCode,
                notification.UserEmail,
                errorText);
        }

        response.EnsureSuccessStatusCode();
    }

    private sealed record SendVerificationEmailRequest(
        string UserEmail,
        string? UserId,
        string Code,
        string? Link);
}
