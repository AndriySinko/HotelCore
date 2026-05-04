using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;







public class ReservationRepository(ApplicationDbContext dbContext)
    : BaseRepository<Reservation>(dbContext), IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<Reservation?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Room)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<Reservation?> GetByIdFullAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<List<Reservation>> FindByGuestNameAsync(string name, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Guest)
            .Where(r => r.Guest != null &&
                (r.Guest.FirstName.Contains(name) || r.Guest.LastName.Contains(name)))
            .ToListAsync(ct);

    public async Task<Reservation?> FindByQrCodeAsync(string code, CancellationToken ct = default)
        => await dbContext.Reservations.FirstOrDefaultAsync(r => r.QrCode == code, ct);

    public async Task<List<Reservation>> GetActiveByGuestIdAsync(Guid guestId, CancellationToken ct = default)
        => await dbContext.Reservations
            .Where(r => r.GuestId == guestId)
            .ToListAsync(ct);

    public async Task<List<(Reservation Reservation, string GuestName, string GuestEmail)>> SearchPublicAsync(string query, CancellationToken ct = default)
    {
        var upper = query.Trim().ToUpper();

        
        var emailMatchIds = await dbContext.Users
            .Where(u => u.NormalizedEmail != null && u.NormalizedEmail.Contains(upper))
            .Select(u => u.Id)
            .ToListAsync(ct);

        var reservations = await dbContext.Reservations
            .Include(r => r.Room)
            .Where(r => r.QrCode == upper || emailMatchIds.Contains(r.GuestId))
            .OrderByDescending(r => r.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

        if (reservations.Count == 0) return [];

        var ids      = reservations.Select(r => r.GuestId).ToHashSet();
        var guestMap = await dbContext.Guests.Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id, ct);
        var userMap  = await dbContext.Users .Where(u => ids.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);

        return reservations.Select(r =>
        {
            userMap.TryGetValue(r.GuestId, out var user);
            var name = guestMap.TryGetValue(r.GuestId, out var guest)
                ? $"{guest.FirstName} {guest.LastName}".Trim()
                : "";
            return ValueTuple.Create(r, name, user?.Email ?? "");
        }).ToList();
    }

    public async Task<(string FirstName, string LastName, string Email, string? Phone)> GetGuestInfoAsync(Guid guestId, CancellationToken ct = default)
    {
        var guest = await dbContext.Guests.FirstOrDefaultAsync(g => g.Id == guestId, ct);
        if (guest != null)
            return (guest.FirstName, guest.LastName, guest.Email ?? "", guest.Phone);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == guestId, ct);
        return ("", "", user?.Email ?? "", null);
    }

    public async Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
            .Where(r => r.Status == ReservationStatus.Reserved || r.Status == ReservationStatus.CheckedIn)
            .OrderBy(r => r.CheckInDate)
            .ToListAsync(ct);

    public async Task<List<(Reservation Reservation, string GuestName, string GuestEmail)>> GetActiveReservationsWithNamesAsync(CancellationToken ct = default)
    {
        var reservations = await dbContext.Reservations
            .Include(r => r.Room)
            .Where(r => r.Status == ReservationStatus.Reserved || r.Status == ReservationStatus.CheckedIn)
            .OrderBy(r => r.CheckInDate)
            .ToListAsync(ct);

        if (reservations.Count == 0) return [];

        var ids      = reservations.Select(r => r.GuestId).ToHashSet();
        var guestMap = await dbContext.Guests.Where(g => ids.Contains(g.Id)).ToDictionaryAsync(g => g.Id, ct);
        var userMap  = await dbContext.Users .Where(u => ids.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);

        return reservations.Select(r =>
        {
            var email = userMap.TryGetValue(r.GuestId, out var u) ? u.Email ?? "" : "";
            var name  = guestMap.TryGetValue(r.GuestId, out var g)
                ? $"{g.FirstName} {g.LastName}".Trim()
                : "";
            return (r, name, email);
        }).ToList();
    }

    public async Task AddAsync(Reservation entity, CancellationToken ct = default)
        => await dbContext.Reservations.AddAsync(entity, ct);

    public void Update(Reservation entity)
        => dbContext.Reservations.Update(entity);

    public async Task SetStatusAsync(Guid id, ReservationStatus status, CancellationToken ct = default)
        => await dbContext.Reservations
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, status)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow), ct);

    public async Task AddPaymentAsync(Payment payment, CancellationToken ct = default)
    {
        
        
        dbContext.ChangeTracker.Clear();
        await dbContext.ReceptionPayments.AddAsync(payment, ct);
    }
}
