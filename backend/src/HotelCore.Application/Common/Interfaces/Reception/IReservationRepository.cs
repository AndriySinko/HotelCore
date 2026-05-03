// This file contains code for IReservationRepository.
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Interfaces.Reception;





public interface IReservationRepository : IBaseRepository<Reservation>
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Reservation?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<Reservation?> GetByIdFullAsync(Guid id, CancellationToken ct = default);
    Task<List<Reservation>> FindByGuestNameAsync(string name, CancellationToken ct = default);
    Task<Reservation?> FindByQrCodeAsync(string code, CancellationToken ct = default);
    Task<List<Reservation>> GetActiveByGuestIdAsync(Guid guestId, CancellationToken ct = default);
    Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken ct = default);
    Task<List<(Reservation Reservation, string GuestName, string GuestEmail)>> GetActiveReservationsWithNamesAsync(CancellationToken ct = default);
    Task<List<(Reservation Reservation, string GuestName, string GuestEmail)>> SearchPublicAsync(string query, CancellationToken ct = default);
    Task<(string FirstName, string LastName, string Email, string? Phone)> GetGuestInfoAsync(Guid guestId, CancellationToken ct = default);
    Task AddAsync(Reservation entity, CancellationToken ct = default);
    void Update(Reservation entity);
    Task SetStatusAsync(Guid id, ReservationStatus status, CancellationToken ct = default);
    Task AddPaymentAsync(Payment payment, CancellationToken ct = default);
}
