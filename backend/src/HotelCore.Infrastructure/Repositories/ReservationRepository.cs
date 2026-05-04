// handles all database queries for the Reservation aggregate
// several variants of GetById exist because different use cases need different related data loaded
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class ReservationRepository(ApplicationDbContext dbContext)
    : BaseRepository<Reservation>(dbContext), IReservationRepository
{
    // minimal load — no navigation properties, used when only the reservation record itself is needed
    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, ct);

    // loads room and payments — used during check-in and check-out flows
    public async Task<Reservation?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Room)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    // full load including guest — used when the receptionist needs to display all details
    public async Task<Reservation?> GetByIdFullAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    // search by partial guest name — used in the reception search box
    public async Task<List<Reservation>> FindByGuestNameAsync(string name, CancellationToken ct = default)
        => await dbContext.Reservations
            .Include(r => r.Guest)
            .Where(r => r.Guest != null &&
                (r.Guest.FirstName.Contains(name) || r.Guest.LastName.Contains(name)))
            .ToListAsync(ct);

    // look up by the QR code printed on the booking confirmation
    public async Task<Reservation?> FindByQrCodeAsync(string code, CancellationToken ct = default)
        => await dbContext.Reservations.FirstOrDefaultAsync(r => r.QrCode == code, ct);

    public async Task<List<Reservation>> GetActiveByGuestIdAsync(Guid guestId, CancellationToken ct = default)
        => await dbContext.Reservations
            .Where(r => r.GuestId == guestId)
            .ToListAsync(ct);

    // public search used on the guest-facing booking lookup page — no auth required
    // supports searching by QR code OR email address, returns at most 20 results
    public async Task<List<(Reservation Reservation, string GuestName, string GuestEmail)>> SearchPublicAsync(string query, CancellationToken ct = default)
    {
        var upper = query.Trim().ToUpper();

        // email lookup needs a separate query because guest email is on the User table, not Reservation
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

        // fetch guest names and emails in two separate queries then join in memory
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

    // returns guest name and email for a given guestId
    // checks the Guest table first, falls back to the base User table for walk-in guests without a full profile
    public async Task<(string FirstName, string LastName, string Email, string? Phone)> GetGuestInfoAsync(Guid guestId, CancellationToken ct = default)
    {
        var guest = await dbContext.Guests.FirstOrDefaultAsync(g => g.Id == guestId, ct);
        if (guest != null)
            return (guest.FirstName, guest.LastName, guest.Email ?? "", guest.Phone);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == guestId, ct);
        return ("", "", user?.Email ?? "", null);
    }

    // returns all reservations that are still active (Reserved or CheckedIn), sorted by check-in date
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

    // bulk status update using ExecuteUpdate — skips loading the entity into memory
    public async Task SetStatusAsync(Guid id, ReservationStatus status, CancellationToken ct = default)
        => await dbContext.Reservations
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, status)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow), ct);

    public async Task AddPaymentAsync(Payment payment, CancellationToken ct = default)
    {
        // clear tracker first to avoid duplicate key conflicts when EF tries to re-attach related entities
        dbContext.ChangeTracker.Clear();
        await dbContext.ReceptionPayments.AddAsync(payment, ct);
    }
}
