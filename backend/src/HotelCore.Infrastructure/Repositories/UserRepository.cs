using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Users;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Persistence;
using HotelCore.Domain.Enums;

namespace HotelCore.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.PhoneNumber == phone, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken) =>
        await dbContext.Users.AddAsync(user, cancellationToken);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Include(u => u.Avatar)
            .Include(u => u.WorkerProfile)
            .Include(u => u.SeekerProfile)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<(User? User, int CompletedOrdersCount, int CreatedOrdersCount)> GetPublicProfileAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Avatar)
            .Include(u => u.WorkerProfile)
            .Include(u => u.SeekerProfile)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            return (null, 0, 0);
        }

        var completedOrdersCount = await dbContext.Offers
            .AsNoTracking()
            .Where(o => o.WorkerProfile.UserId == id && o.IsAccepted && o.WorkRequest.Status == WorkRequestStatus.Completed)
            .CountAsync(cancellationToken);

        var createdOrdersCount = await dbContext.WorkRequests
            .AsNoTracking()
            .Where(wr => wr.SeekerProfile.UserId == id)
            .CountAsync(cancellationToken);

        return (user, completedOrdersCount, createdOrdersCount);
    }
}