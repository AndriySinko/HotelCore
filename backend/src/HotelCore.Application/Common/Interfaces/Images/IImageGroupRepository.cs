using HotelCore.Domain.Entities.Images;

namespace HotelCore.Application.Common.Interfaces.Images;

public interface IImageGroupRepository
{
    Task AddAsync(MyImageGroup imageGroup, CancellationToken cancellationToken);
}
