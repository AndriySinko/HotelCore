namespace HotelCore.Application.EmailVerification.Models;

public sealed record EmailVerificationSendResult(DateTimeOffset ExpiresAt);
