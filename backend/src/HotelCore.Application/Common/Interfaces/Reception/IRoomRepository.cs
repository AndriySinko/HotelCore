// This file contains code for IRoomRepository.
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Interfaces.Reception;





public interface IRoomRepository : IBaseRepository<Room>
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Room>> GetAllAsync(CancellationToken ct = default);
    Task<List<Room>> GetAvailableRoomsAsync(DateTime from, DateTime to, RoomType? type = null, CancellationToken ct = default);
    Task<List<Room>> GetAlternativeRoomsAsync(RoomType roomType, DateTime date, CancellationToken ct = default);
    void Update(Room entity);
    Task SetStatusAsync(Guid id, RoomStatus status, CancellationToken ct = default);
}
