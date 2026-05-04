// data access for rooms — availability queries are the most complex part here
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class RoomRepository(ApplicationDbContext dbContext)
    : BaseRepository<Room>(dbContext), IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<List<Room>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.Rooms.OrderBy(r => r.RoomNumber).ToListAsync(ct);

    // finds rooms that are Available AND have no overlapping reservation in the requested period
    // both conditions must be true — a room in Occupied status wont show up even if dates dont conflict
    public async Task<List<Room>> GetAvailableRoomsAsync(
        DateTime from, DateTime to, RoomType? type = null, CancellationToken ct = default)
    {
        var query = dbContext.Rooms
            .Where(r => r.Status == RoomStatus.Available)
            .Where(r => !dbContext.Reservations.Any(res =>
                res.RoomId == r.Id &&
                res.Status != ReservationStatus.Cancelled &&
                res.CheckInDate < to &&
                res.CheckOutDate > from));

        if (type.HasValue)
            query = query.Where(r => r.RoomType == type.Value);

        return await query.ToListAsync(ct);
    }

    // used during check-in when the originally reserved room is not ready
    // finds other rooms of the same type that are currently Available
    public async Task<List<Room>> GetAlternativeRoomsAsync(
        RoomType roomType, DateTime date, CancellationToken ct = default)
        => await dbContext.Rooms
            .Where(r => r.RoomType == roomType && r.Status == RoomStatus.Available)
            .ToListAsync(ct);

    public void Update(Room entity)
        => dbContext.Rooms.Update(entity);

    // direct update without loading the entity — more efficient than fetch-modify-save
    public async Task SetStatusAsync(Guid id, RoomStatus status, CancellationToken ct = default)
        => await dbContext.Rooms
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, status)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow), ct);
}
