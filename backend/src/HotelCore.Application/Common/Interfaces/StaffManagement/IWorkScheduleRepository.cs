// This file contains code for IWorkScheduleRepository.
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Application.Common.Interfaces.StaffManagement;

public interface IWorkScheduleRepository : IBaseRepository<WorkSchedule>
{
    Task<WorkSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WorkSchedule?> GetByIdWithShiftsAsync(Guid id, CancellationToken ct = default);
    Task<WorkSchedule?> GetLatestPublishedByStaffIdAsync(Guid staffId, CancellationToken ct = default);
    
    Task<WorkSchedule?> GetByPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<List<WorkSchedule>> GetAllWithShiftsAsync(CancellationToken ct = default);
    Task AddAsync(WorkSchedule entity, CancellationToken ct = default);
    void Update(WorkSchedule entity);
}
