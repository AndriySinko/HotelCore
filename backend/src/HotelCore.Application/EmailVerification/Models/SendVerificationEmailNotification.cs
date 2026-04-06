namespace HotelCore.Application.EmailVerification.Models;

public sealed record SendVerificationEmailNotification(
    string UserEmail,
    string? UserId,
    string Code,
    string? Link);
