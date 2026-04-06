namespace HotelCore.Application.EmailVerification.Models;

public sealed class EmailVerificationRecord
{
    public string Email { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
