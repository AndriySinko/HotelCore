using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Orders;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.CommandHandlers;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Queries;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.QueryHandlers;

public class GetOrderHandler(IApplicationDbContext db)
    : IRequestHandler<GetOrderQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.OrderId);

        var items = order.OrderItems.Select(i => new OrderItemResponse(
            i.Id,
            i.Product?.Name ?? "Unknown",
            i.PricePerUnit,
            i.Quantity,
            i.SpecialRequest)).ToList();

        return new OrderResponse(
            order.Id,
            CreateOrderHandler.MapStatus(order.Status),
            items,
            items.Sum(i => i.PricePerUnit * i.Quantity),
            order.Payment is not null ? CreateOrderHandler.MapPaymentMethod(order.Payment.Method) : "unknown",
            order.CreatedAt);
    }
}
