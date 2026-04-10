// This file contains code for IWorkerPortfolioItemRepository.
using HotelCore.Application.Common.Models;
using HotelCore.Application.MastersProjects.Models;
using HotelCore.Domain.Entities.Workers;

namespace HotelCore.Application.Common.Interfaces.WorkersPortfolio;

public interface IWorkerPortfolioItemRepository :  IBaseRepository<WorkerPortfolioItem>
{
    Task AddAsync(WorkerPortfolioItem item, CancellationToken cancellationToken);
    Task<WorkerPortfolioItem?> GetByIdAsync(Guid id,  CancellationToken cancellationToken);
    Task<WorkerPortfolioItem?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkerPortfolioItem>> GetMasterProjectsAsync(
        MasterProjectFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken);
    public void Update(WorkerPortfolioItem item);
   
}
