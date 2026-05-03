// This file contains code for IGuestRepository.
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Application.Common.Interfaces.Reception;

public interface IGuestRepository : IBaseRepository<Guest>
{
    Task<Guest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guest?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default);
    Task<Guest?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<Guid?> FindUserIdByEmailAsync(string email, CancellationToken ct = default);
    void Update(Guest entity);
}
