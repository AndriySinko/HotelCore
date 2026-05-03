using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class CleaningTaskRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<CleaningTask>(applicationDbContext), ICleaningTaskRepository
{
    public async Task<CleaningTask?> GetByIdAsync(Guid identifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Include(task => task.Room)
            .FirstOrDefaultAsync(task => task.Id == identifier, cancellationToken);

    public async Task<List<CleaningTask>> GetByRoomAsync(Guid roomIdentifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Where(task => task.RoomId == roomIdentifier)
            .ToListAsync(cancellationToken);

    public async Task<List<CleaningTask>> GetActiveByReservationAsync(Guid reservationIdentifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Where(task => task.ReservationId == reservationIdentifier &&
                task.Status != CleaningTaskStatus.Completed &&
                task.Status != CleaningTaskStatus.Cancelled)
            .ToListAsync(cancellationToken);

    public async Task<List<CleaningTask>> GetPendingTasksAsync(CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Include(task => task.Room)
            .Include(task => task.AssignedStaff)
            .Where(task => task.Status == CleaningTaskStatus.Requested || task.Status == CleaningTaskStatus.Assigned)
            .ToListAsync(cancellationToken);

    public async Task<List<CleaningTask>> GetTasksForStaffAsync(Guid staffIdentifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Include(task => task.Room)
            .Include(task => task.AssignedStaff)
            .Where(task => task.AssignedStaffId == staffIdentifier)
            .OrderBy(task => task.Priority)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(CleaningTask cleaningTask, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks.AddAsync(cleaningTask, cancellationToken);

    public void Update(CleaningTask cleaningTask)
        => applicationDbContext.CleaningTasks.Update(cleaningTask);
}
