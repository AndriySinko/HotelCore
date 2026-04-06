using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class BaseRepository<T>
{
    public readonly ApplicationDbContext dbContext;
    
    public BaseRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Delete(T entity)
    {
        dbContext.Remove(entity);
    }
}