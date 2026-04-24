using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Orders;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.CommandHandlers;

public class CancelOrderHandler(IApplicationDbContext db, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.Status != OrderStatus.Received)
            throw new BadRequestException("Only orders with status 'received' can be cancelled.");

        order.Status = OrderStatus.Canceled;
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
