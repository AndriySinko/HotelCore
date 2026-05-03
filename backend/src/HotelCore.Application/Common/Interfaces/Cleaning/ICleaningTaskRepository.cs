// This file contains code for ICleaningTaskRepository.
using HotelCore.Domain.Entities.Cleaning;

namespace HotelCore.Application.Common.Interfaces.Cleaning;

public interface ICleaningTaskRepository : IBaseRepository<CleaningTask>
{
    Task<CleaningTask?> GetByIdAsync(Guid identifier, CancellationToken cancellationToken = default);
    Task<List<CleaningTask>> GetByRoomAsync(Guid roomIdentifier, CancellationToken cancellationToken = default);
    Task<List<CleaningTask>> GetActiveByReservationAsync(Guid reservationIdentifier, CancellationToken cancellationToken = default);
    Task<List<CleaningTask>> GetPendingTasksAsync(CancellationToken cancellationToken = default);
    Task<List<CleaningTask>> GetTasksForStaffAsync(Guid staffIdentifier, CancellationToken cancellationToken = default);
    Task AddAsync(CleaningTask cleaningTask, CancellationToken cancellationToken = default);
    void Update(CleaningTask cleaningTask);
}
