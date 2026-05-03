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

    public async Task<List<Room>> GetAlternativeRoomsAsync(
        RoomType roomType, DateTime date, CancellationToken ct = default)
        => await dbContext.Rooms
            .Where(r => r.RoomType == roomType && r.Status == RoomStatus.Available)
            .ToListAsync(ct);

    public void Update(Room entity)
        => dbContext.Rooms.Update(entity);

    public async Task SetStatusAsync(Guid id, RoomStatus status, CancellationToken ct = default)
        => await dbContext.Rooms
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, status)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow), ct);
}
