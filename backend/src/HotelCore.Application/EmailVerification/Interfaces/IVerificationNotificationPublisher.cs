using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Application.EmailVerification.Interfaces;

public interface IVerificationNotificationPublisher
{
    Task PublishSendVerificationEmailAsync(SendVerificationEmailNotification notification, CancellationToken cancellationToken);
}
