namespace HotelCore.Application.EmailVerification.Options;

public sealed class EmailVerificationOptions
{
    public int CodeLength { get; set; } = 6;
    public int MaxAttempts { get; set; } = 5;
    public int TimeToLiveHours { get; set; } = 3;
    public string? VerificationBaseUrl { get; set; }
}
