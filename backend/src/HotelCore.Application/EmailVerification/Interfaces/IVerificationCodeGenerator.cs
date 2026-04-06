namespace HotelCore.Application.EmailVerification.Interfaces;

public interface IVerificationCodeGenerator
{
    string Generate(int digits);
}
