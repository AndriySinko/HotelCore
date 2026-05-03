// This file contains code for IWorkRequestRepository.
using HotelCore.Application.Common.Models;
using HotelCore.Application.Communication.WorkRequests.Models;
using HotelCore.Domain.Entities.Communication;

namespace HotelCore.Application.Common.Interfaces.WorkRequests;

public interface IWorkRequestRepository : IBaseRepository<WorkRequest>
{
    Task<WorkRequest?> GetByIdAsync(Guid workRequestId, CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkRequest>> GetWorkRequestsAsync(
        WorkRequestsFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken);

    void Add(WorkRequest workRequest);
    void Update(WorkRequest workRequest);
    
}
