using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;

namespace HotelCore.Application.Common.Authorization;

public class OrderAccessPolicy : IOrderAccessPolicy
{
    public bool IsOwner(Order order, Guid? userId)
        => userId.HasValue && order.CreatedByUserId == userId.Value;

    public bool IsAuthorizedParty(Order order, Guid? userId, string? guestToken)
        => IsOwner(order, userId)
           || (userId.HasValue && order.Client?.UserId == userId.Value)
           || (!string.IsNullOrEmpty(guestToken)
               && order.Client?.UserId is null
               && order.Client?.GuestAccessToken == guestToken);
}
