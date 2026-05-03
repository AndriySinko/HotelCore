// data access layer for cleaning tasks — all database queries for this module go through here
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class CleaningTaskRepository(ApplicationDbContext applicationDbContext)
    : BaseRepository<CleaningTask>(applicationDbContext), ICleaningTaskRepository
{
    // loads the task with its room — used when the handler needs to check room details
    public async Task<CleaningTask?> GetByIdAsync(Guid identifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Include(task => task.Room)
            .FirstOrDefaultAsync(task => task.Id == identifier, cancellationToken);

    // returns all tasks for a given room — used to detect duplicate active tasks before creating a new one
    public async Task<List<CleaningTask>> GetByRoomAsync(Guid roomIdentifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Where(task => task.RoomId == roomIdentifier)
            .ToListAsync(cancellationToken);

    // returns only non-terminal tasks for a reservation — completed and cancelled ones are excluded
    // used to check if a reservation still has pending cleaning work before checkout
    public async Task<List<CleaningTask>> GetActiveByReservationAsync(Guid reservationIdentifier, CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Where(task => task.ReservationId == reservationIdentifier &&
                task.Status != CleaningTaskStatus.Completed &&
                task.Status != CleaningTaskStatus.Cancelled)
            .ToListAsync(cancellationToken);

    // returns tasks in Requested or Assigned status — these are what supervisors see in the assignment panel
    // includes room and staff so the frontend can show room number and assigned worker name
    public async Task<List<CleaningTask>> GetPendingTasksAsync(CancellationToken cancellationToken = default)
        => await applicationDbContext.CleaningTasks
            .Include(task => task.Room)
            .Include(task => task.AssignedStaff)
            .Where(task => task.Status == CleaningTaskStatus.Requested || task.Status == CleaningTaskStatus.Assigned)
            .ToListAsync(cancellationToken);

    // returns all tasks assigned to a specific worker, sorted by priority (1 = most urgent)
    // this is what a cleaning worker sees when they open the app
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
