using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Application.EmailVerification.Interfaces;

public interface IEmailVerificationStore
{
    Task<EmailVerificationRecord?> GetAsync(string email, CancellationToken cancellationToken);
    Task SaveAsync(EmailVerificationRecord record, TimeSpan ttl, CancellationToken cancellationToken);
    Task<long> IncrementAttemptsAsync(string email, CancellationToken cancellationToken);
    Task DeleteAsync(string email, CancellationToken cancellationToken);
}
