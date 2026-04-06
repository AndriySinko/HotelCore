namespace HotelCore.Application.EmailVerification.Models;

public enum EmailVerificationStatus
{
    Success = 0,
    NotFound = 1,
    Expired = 2,
    TooManyAttempts = 3,
    InvalidCode = 4
}
