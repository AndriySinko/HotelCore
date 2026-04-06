namespace HotelCore.Application.Common.Interfaces;

public interface IBaseRepository<T>
{
    public void Delete(T entity);
}