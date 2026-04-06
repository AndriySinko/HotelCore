using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Models;
using HotelCore.Application.EmailVerification.Options;

namespace HotelCore.Application.EmailVerification.Commands.SendEmailVerification;

public sealed class SendEmailVerificationCommandHandler(
    IEmailVerificationStore store,
    IVerificationCodeGenerator codeGenerator,
    IVerificationNotificationPublisher notificationPublisher,
    IOptions<EmailVerificationOptions> options,
    ILogger<SendEmailVerificationCommandHandler> logger)
    : IRequestHandler<SendEmailVerificationCommand, EmailVerificationSendResult>
{
    private readonly EmailVerificationOptions _options = options.Value;

    public async Task<EmailVerificationSendResult> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var email = NormalizeEmail(request.Email);
        var code = codeGenerator.Generate(Math.Max(1, _options.CodeLength));
        var token = Guid.NewGuid().ToString("N");
        var ttl = TimeSpan.FromHours(Math.Max(1, _options.TimeToLiveHours));
        var expiresAt = now.Add(ttl);

        var record = new EmailVerificationRecord
        {
            Email = email,
            UserId = request.UserId,
            Code = code,
            Token = token,
            Attempts = 0,
            CreatedAt = now,
            ExpiresAt = expiresAt
        };

        await store.SaveAsync(record, ttl, cancellationToken);

        var link = BuildLink(email, token, request.BaseUrl);
        await notificationPublisher.PublishSendVerificationEmailAsync(
            new SendVerificationEmailNotification(email, request.UserId, code, link),
            cancellationToken);

        logger.LogInformation("Verification code issued for {Email}.", email);

        return new EmailVerificationSendResult(expiresAt);
    }

    private string? BuildLink(string email, string token, string? baseUrl)
    {
        var resolvedBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
            ? _options.VerificationBaseUrl
            : baseUrl;

        if (string.IsNullOrWhiteSpace(resolvedBaseUrl))
        {
            return null;
        }

        var trimmedBaseUrl = resolvedBaseUrl.TrimEnd('/');
        return $"{trimmedBaseUrl}/api/EmailVerification/confirm?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
