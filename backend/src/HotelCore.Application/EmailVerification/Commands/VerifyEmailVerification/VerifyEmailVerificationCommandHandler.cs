using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Models;
using HotelCore.Application.EmailVerification.Options;

namespace HotelCore.Application.EmailVerification.Commands.VerifyEmailVerification;

public sealed class VerifyEmailVerificationCommandHandler(
    IEmailVerificationStore store,
    IUserEmailConfirmationService userEmailConfirmationService,
    IOptions<EmailVerificationOptions> options,
    ILogger<VerifyEmailVerificationCommandHandler> logger)
    : IRequestHandler<VerifyEmailVerificationCommand, EmailVerificationResult>
{
    private readonly EmailVerificationOptions _options = options.Value;

    public async Task<EmailVerificationResult> Handle(VerifyEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        var record = await store.GetAsync(email, cancellationToken);

        if (record is null)
        {
            return new EmailVerificationResult(EmailVerificationStatus.NotFound, null);
        }

        var now = DateTimeOffset.UtcNow;
        if (record.ExpiresAt <= now)
        {
            await store.DeleteAsync(email, cancellationToken);
            return new EmailVerificationResult(EmailVerificationStatus.Expired, null);
        }

        var maxAttempts = Math.Max(1, _options.MaxAttempts);
        if (record.Attempts >= maxAttempts)
        {
            return new EmailVerificationResult(EmailVerificationStatus.TooManyAttempts, 0);
        }

        var code = request.Code?.Trim();
        var token = request.Token?.Trim();

        var matchesCode = !string.IsNullOrWhiteSpace(code)
            && string.Equals(record.Code, code, StringComparison.Ordinal);
        var matchesToken = !string.IsNullOrWhiteSpace(token)
            && string.Equals(record.Token, token, StringComparison.Ordinal);

        if (matchesCode || matchesToken)
        {
            await store.DeleteAsync(email, cancellationToken);
            await userEmailConfirmationService.ConfirmAsync(record.Email, record.UserId, cancellationToken);

            logger.LogInformation("Email verified for {Email}.", email);
            return new EmailVerificationResult(EmailVerificationStatus.Success, null);
        }

        var attempts = await store.IncrementAttemptsAsync(email, cancellationToken);
        if (attempts >= maxAttempts)
        {
            return new EmailVerificationResult(EmailVerificationStatus.TooManyAttempts, 0);
        }

        var attemptsLeft = Math.Max(0, maxAttempts - (int)attempts);
        return new EmailVerificationResult(EmailVerificationStatus.InvalidCode, attemptsLeft);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
