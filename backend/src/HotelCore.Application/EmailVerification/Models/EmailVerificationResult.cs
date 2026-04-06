namespace HotelCore.Application.EmailVerification.Models;

public sealed record EmailVerificationResult(EmailVerificationStatus Status, int? AttemptsLeft);
