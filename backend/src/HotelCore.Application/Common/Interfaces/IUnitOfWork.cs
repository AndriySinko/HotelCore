namespace HotelCore.Application.Common.Interfaces;



public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
    