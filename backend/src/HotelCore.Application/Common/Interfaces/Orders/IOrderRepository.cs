// This file contains code for IOrderRepository.
using HotelCore.Application.Common.Models;
using HotelCore.Application.Orders.Models;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Orders;

namespace HotelCore.Application.Common.Interfaces.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MyImageGroup>> GetOrderImagesAsync(
        Guid orderId,
        PageRequest pageRequest,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> GetOrdersAsync(
        OrdersFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> GetSubOrdersAsync(Guid parentOrderId, CancellationToken cancellationToken);

    Task<int> MoveGuestOrdersToUserAsync(string guestAccessToken, Guid userId, CancellationToken cancellationToken);

    void Add(Order order);
    void Update(Order order);
}
