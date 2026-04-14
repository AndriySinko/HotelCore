using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class ShiftRepository(ApplicationDbContext dbContext)
    : BaseRepository<Shift>(dbContext), IShiftRepository
{
    public async Task<Shift?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Shifts.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<Shift>> GetByScheduleAsync(Guid scheduleId, CancellationToken ct = default)
        => await dbContext.Shifts.Where(s => s.WorkScheduleId == scheduleId).ToListAsync(ct);

    public async Task<List<Shift>> GetByStaffAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default)
        => await dbContext.Shifts
            .Where(s => s.StaffMemberId == staffId && s.Date >= from && s.Date <= to)
            .ToListAsync(ct);

    public async Task AddAsync(Shift shift, CancellationToken ct = default)
        => await dbContext.Shifts.AddAsync(shift, ct);

    public async Task DeleteByScheduleAsync(Guid scheduleId, CancellationToken ct = default)
    {
        var shifts = await dbContext.Shifts
            .Where(s => s.WorkScheduleId == scheduleId)
            .ToListAsync(ct);
        dbContext.Shifts.RemoveRange(shifts);
    }

    public void Update(Shift entity) => dbContext.Shifts.Update(entity);
}
