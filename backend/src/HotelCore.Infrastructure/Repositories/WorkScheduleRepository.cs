using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class WorkScheduleRepository(ApplicationDbContext dbContext)
    : BaseRepository<WorkSchedule>(dbContext), IWorkScheduleRepository
{
    public async Task<WorkSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.WorkSchedules.FirstOrDefaultAsync(s => s.Id == id, ct);

    
    public async Task<WorkSchedule?> GetByIdWithShiftsAsync(Guid id, CancellationToken ct = default)
        => await dbContext.WorkSchedules
            .Include(s => s.Shifts)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<WorkSchedule?> GetLatestPublishedByStaffIdAsync(Guid staffId, CancellationToken ct = default)
        => await dbContext.WorkSchedules
            .Include(s => s.Shifts)
            .Where(s => s.Status == ScheduleStatus.Published && s.Shifts.Any(sh => sh.StaffMemberId == staffId))
            .OrderByDescending(s => s.PeriodStart)
            .FirstOrDefaultAsync(ct);

    public async Task<WorkSchedule?> GetByPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default)
        => await dbContext.WorkSchedules
            .FirstOrDefaultAsync(s => s.PeriodStart == start && s.PeriodEnd == end, ct);

    public async Task<List<WorkSchedule>> GetAllWithShiftsAsync(CancellationToken ct = default)
        => await dbContext.WorkSchedules
            .Include(s => s.Shifts).ThenInclude(sh => sh.AssignedEmployee)
            .OrderByDescending(s => s.PeriodStart)
            .ToListAsync(ct);

    public async Task AddAsync(WorkSchedule entity, CancellationToken ct = default)
        => await dbContext.WorkSchedules.AddAsync(entity, ct);

    public void Update(WorkSchedule entity)
        => dbContext.WorkSchedules.Update(entity);
}
