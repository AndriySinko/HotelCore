using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Domain.Entities.Images;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class ImageGroupRepository(ApplicationDbContext db) : IImageGroupRepository
{
    public async Task AddAsync(MyImageGroup imageGroup, CancellationToken cancellationToken)
    {
        await db.MyImageGroups.AddAsync(imageGroup, cancellationToken);
    }
}
