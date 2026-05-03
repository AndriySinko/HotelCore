using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class StaffRepository(ApplicationDbContext dbContext)
    : BaseRepository<StaffMember>(dbContext), IStaffRepository
{
    public async Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<StaffMember>> GetByDepartmentAsync(string department, CancellationToken ct = default)
        => await dbContext.StaffMembers
            .Where(s => s.Department == department)
            .ToListAsync(ct);

    
    public async Task<List<StaffMember>> GetAvailableStaffAsync(DateTime date, string role, CancellationToken ct = default)
        => await dbContext.StaffMembers
            .Where(s => s.Position == role)
            .Where(s => !dbContext.Shifts.Any(sh =>
                sh.StaffMemberId == s.Id &&
                sh.Date.Date == date.Date))
            .ToListAsync(ct);

    public async Task<List<StaffMember>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.StaffMembers.ToListAsync(ct);
}
