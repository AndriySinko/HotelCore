// This file contains code for IUnitOfWork.
namespace HotelCore.Application.Common.Interfaces;



public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void MarkUnchanged<T>(T entity) where T : class;
}
    