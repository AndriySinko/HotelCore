using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class GuestRepository(ApplicationDbContext dbContext)
    : BaseRepository<Guest>(dbContext), IGuestRepository
{
    public async Task<Guest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Guests.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Guest?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Guests.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<Guest?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await dbContext.Guests
            .FirstOrDefaultAsync(g => g.NormalizedEmail == email.ToUpperInvariant(), ct);

    
    public async Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpperInvariant(), ct);
        return user?.Id;
    }

    public void Update(Guest entity)
        => dbContext.Guests.Update(entity);
}
