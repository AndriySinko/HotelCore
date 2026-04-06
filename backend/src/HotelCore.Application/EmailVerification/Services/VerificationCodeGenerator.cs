using System.Security.Cryptography;
using HotelCore.Application.EmailVerification.Interfaces;

namespace HotelCore.Application.EmailVerification.Services;

public sealed class VerificationCodeGenerator : IVerificationCodeGenerator
{
    public string Generate(int digits)
    {
        if (digits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be greater than zero.");
        }

        var maxExclusive = (int)Math.Pow(10, digits);
        var value = RandomNumberGenerator.GetInt32(0, maxExclusive);
        return value.ToString($"D{digits}");
    }
}
