// This file contains code for IShiftRepository.
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Application.Common.Interfaces.StaffManagement;

public interface IShiftRepository : IBaseRepository<Shift>
{
    Task<Shift?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Shift>> GetByScheduleAsync(Guid scheduleId, CancellationToken ct = default);
    Task<List<Shift>> GetByStaffAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
    Task AddAsync(Shift shift, CancellationToken ct = default);
    Task DeleteByScheduleAsync(Guid scheduleId, CancellationToken ct = default);
    void Update(Shift entity);
}
