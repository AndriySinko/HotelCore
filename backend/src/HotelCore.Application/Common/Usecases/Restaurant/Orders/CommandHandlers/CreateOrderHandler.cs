using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Orders;
using HotelCore.Domain.Exceptions;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;
using HotelCore.Domain.Entities.Users.Restaurant;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.CommandHandlers;

/// <summary>
/// Creates a new room-service order from the guest's cart contents.
/// Prices are snapshotted at creation time — product prices can change later
/// without affecting the amounts on existing orders.
/// </summary>
public class CreateOrderHandler(IApplicationDbContext db, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new BadRequestException("Order must contain at least one item.");

        // TryParse returns Guid.Empty on failure; we treat that as "no linked guest" below.
        // The endpoint is [Authorize], so the token is always present — this guards against
        // edge cases where the sub claim isn't a valid Guid (e.g. demo tokens).
        _ = Guid.TryParse(currentUser.UserId, out var guestId);

        var productIds = request.Items.Select(i => i.ProductId).ToList();

        // Batch-fetch all products upfront to avoid N+1 queries during item creation.
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
                PricePerUnit = product.Price,  // snapshot — see class summary
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

    // Mobile expects snake_case strings; the enum values are kept internal.
    internal static string MapStatus(OrderStatus s) => s switch
    {
        OrderStatus.Received  => "received",
        OrderStatus.Preparaing => "preparing",
        OrderStatus.OnTheWay  => "on_the_way",
        OrderStatus.Delivered => "delivered",
        OrderStatus.Canceled  => "cancelled",
        _                     => "in_progress",
    };

    internal static string MapPaymentMethod(PaymentMethod m) => m switch
    {
        PaymentMethod.RoomBill      => "room_bill",
        PaymentMethod.OnlinePayment => "online_payment",
        _                           => "unknown",
    };
}
