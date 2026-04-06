using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Extensions;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Application.Common.Models;
using HotelCore.Application.MastersProjects.Models;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class WorkerPortfolioItemRepository(ApplicationDbContext dbContext) : 
    BaseRepository<WorkerPortfolioItem>(dbContext),
    IWorkerPortfolioItemRepository
{
    public async Task AddAsync(WorkerPortfolioItem item, CancellationToken cancellationToken)
    {
        await dbContext.WorkerPortfolioItems.AddAsync(item, cancellationToken);
    }

    public void Update(WorkerPortfolioItem item)
    {
        dbContext.WorkerPortfolioItems.Update(item);
    }

    public async Task<WorkerPortfolioItem?> GetByIdAsync(Guid workerPortfolioItemId, CancellationToken cancellationToken)
    {
        return await dbContext.WorkerPortfolioItems
            .Include(wpi => wpi.WorkerProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(wpi => wpi.Id == workerPortfolioItemId, cancellationToken);
    }

    public async Task<WorkerPortfolioItem?> GetByIdWithImagesAsync(Guid workerPortfolioItemId, CancellationToken cancellationToken)
    {
        return await dbContext.WorkerPortfolioItems
            .Include(wpi => wpi.Images)
            .ThenInclude(group => group.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(wpi => wpi.Id == workerPortfolioItemId, cancellationToken);
    }
    
    public async Task<IReadOnlyList<WorkerPortfolioItem>> GetMasterProjectsAsync(
        MasterProjectFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = dbContext.WorkerPortfolioItems
            .AsNoTracking()
            .Include(wpi => wpi.Images)
            .ThenInclude(group => group.Images)
            .AsQueryable();

        if (filter.WorkerProfileId.HasValue)
            query = query.Where(wpi => wpi.WorkerProfileId == filter.WorkerProfileId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search.Trim()}%";
            query = query.Where(wpi =>
                EF.Functions.Like(wpi.Title, search));
        }
        query = query.OrderByDescending(wpi => wpi.CreatedAt);

        return await query
            .Paginate(pageRequest)
            .ToListAsync(cancellationToken);
    }
}


