using HotelCore.Domain.Entities.Users;

namespace HotelCore.Application.Users;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(
        string email, 
        CancellationToken cancellationToken = default);
    
    Task<bool> PhoneExistsAsync(
        string phone, 
        CancellationToken cancellationToken = default);
    
    Task AddAsync(
        User user, 
        CancellationToken cancellationToken = default);
    
    Task<User?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
    
    Task<List<User>> GetPagedAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    Task<(User? User, int CompletedOrdersCount, int CreatedOrdersCount)> GetPublicProfileAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}
