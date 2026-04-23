using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Orders;
using HotelCore.Domain.Exceptions;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;
using HotelCore.Domain.Entities.Users.Restaurant;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.CommandHandlers;

public class CreateOrderHandler(IApplicationDbContext db, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new BadRequestException("Order must contain at least one item.");

        _ = Guid.TryParse(currentUser.UserId, out var guestId);

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var order = new Order
        {
            Status = OrderStatus.Received,
            GuestId = guestId == Guid.Empty ? null : guestId,
        };

        foreach (var item in request.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException("Product", item.ProductId);

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                PricePerUnit = product.Price,
                Quantity = item.Quantity,
                SpecialRequest = item.SpecialRequest,
            });
        }

        order.Payment = new Payment { Method = request.PaymentMethod };

        db.Orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(order, products);
    }

    private static OrderResponse MapToResponse(Order order, Dictionary<Guid, Product> products)
    {
        var items = order.OrderItems.Select(i => new OrderItemResponse(
            i.Id,
            products.TryGetValue(i.ProductId, out var p) ? p.Name : "Unknown",
            i.PricePerUnit,
            i.Quantity,
            i.SpecialRequest)).ToList();

        return new OrderResponse(
            order.Id,
            MapStatus(order.Status),
            items,
            items.Sum(i => i.PricePerUnit * i.Quantity),
            MapPaymentMethod(order.Payment!.Method),
            order.CreatedAt);
    }

    internal static string MapStatus(OrderStatus s) => s switch
    {
        OrderStatus.Received => "received",
        OrderStatus.Preparaing => "preparing",
        OrderStatus.OnTheWay => "on_the_way",
        OrderStatus.Delivered => "delivered",
        OrderStatus.Canceled => "cancelled",
        _ => "in_progress",
    };

    internal static string MapPaymentMethod(PaymentMethod m) => m switch
    {
        PaymentMethod.RoomBill => "room_bill",
        PaymentMethod.OnlinePayment => "online_payment",
        _ => "unknown",
    };
}
