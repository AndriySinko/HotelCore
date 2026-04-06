using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Extensions;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Communication.WorkRequests.Models;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class WorkRequestRepository(ApplicationDbContext db) : BaseRepository<WorkRequest>(db), IWorkRequestRepository
{
    public async Task<WorkRequest?> GetByIdAsync(Guid workRequestId, CancellationToken cancellationToken)
    {
        return await db.WorkRequests
            .Include(wr => wr.SeekerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(wr => wr.Id == workRequestId, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkRequest>> GetWorkRequestsAsync(
        WorkRequestsFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = db.WorkRequests
            .AsNoTracking()
            .AsQueryable();

        if (filter.SeekerProfileId.HasValue)
            query = query.Where(wr => wr.SeekerProfileId == filter.SeekerProfileId.Value);

        if (filter.SeekerUserId.HasValue)
            query = query.Where(wr => wr.SeekerProfile.UserId == filter.SeekerUserId.Value);

        if (filter.LocationId.HasValue)
            query = query.Where(wr => wr.LocationId == filter.LocationId.Value);

        if (filter.PreferredDate.HasValue)
        {
            var preferredDate = filter.PreferredDate.Value.Date;
            query = query.Where(wr => wr.PreferredDate.HasValue && wr.PreferredDate.Value.Date == preferredDate);
        }

        if (filter.MinBudget.HasValue)
            query = query.Where(wr => wr.Budget.HasValue && wr.Budget.Value >= filter.MinBudget.Value);

        if (filter.MaxBudget.HasValue)
            query = query.Where(wr => wr.Budget.HasValue && wr.Budget.Value <= filter.MaxBudget.Value);

        if (filter.CategoryIds is { Count: > 0 })
            query = query.Where(wr => filter.CategoryIds.Contains(wr.CategoryId));

        if (filter.Status.HasValue)
            query = query.Where(wr => wr.Status == filter.Status.Value);

        if (filter.ExcludeStatus.HasValue)
            query = query.Where(wr => wr.Status != filter.ExcludeStatus.Value);

        query = query
            .OrderBy(wr => wr.PreferredDate == null)
            .ThenBy(wr => wr.PreferredDate)
            .ThenBy(wr => wr.Budget == null)
            .ThenBy(wr => wr.Budget);

        return await query
            .Paginate(pageRequest)
            .ToListAsync(cancellationToken);
    }

    public void Add(WorkRequest workRequest)
    {
        db.WorkRequests.Add(workRequest);
    }

    public void Update(WorkRequest workRequest)
    {
        db.WorkRequests.Update(workRequest);
    }
}
