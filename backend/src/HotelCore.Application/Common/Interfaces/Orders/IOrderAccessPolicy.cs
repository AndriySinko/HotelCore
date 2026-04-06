using HotelCore.Domain.Entities.Orders;

namespace HotelCore.Application.Common.Interfaces.Orders;

public interface IOrderAccessPolicy
{
    bool IsOwner(Order order, Guid? userId);
    bool IsAuthorizedParty(Order order, Guid? userId, string? guestToken);
}
