namespace HotelCore.Application.EmailVerification.Interfaces;

public interface IUserEmailConfirmationService
{
    Task ConfirmAsync(string email, string? userId, CancellationToken cancellationToken);
}
