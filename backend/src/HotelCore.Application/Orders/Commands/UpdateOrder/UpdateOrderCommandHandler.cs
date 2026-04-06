using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(
    IOrderRepository orderRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork,
    IOrderAccessPolicy accessPolicy,
    ILogger<UpdateOrderCommandHandler> logger)
    : IRequestHandler<UpdateOrderCommand>
{
    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating order {OrderId} by user {UserId}", request.OrderId, request.CurrentUserId);
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (!accessPolicy.IsOwner(order, request.CurrentUserId))
        {
             throw new ForbiddenException("You do not have permission to perform this action");
        }

        var changes = new Dictionary<string, (object? OldValue, object? NewValue)>();

        if (request.Title is not null && request.Title != order.Title)
        {
            logger.LogDebug("Title change detected for order {OrderId}: '{OldTitle}' -> '{NewTitle}'", request.OrderId, order.Title, request.Title);
            changes[nameof(Order.Title)] = (order.Title, request.Title);
            order.Title = request.Title;
        }

        if (request.Description != order.Description)
        {
            logger.LogDebug("Description change detected for order {OrderId}", request.OrderId);
            changes[nameof(Order.Description)] = (order.Description, request.Description);
            order.Description = request.Description;
        }

        if (request.Price != order.Price)
        {
            logger.LogDebug("Price change detected for order {OrderId}: {OldPrice} -> {NewPrice}", request.OrderId, order.Price, request.Price);
            changes[nameof(Order.Price)] = (order.Price, request.Price);
            order.Price = request.Price;
        }

        if (request.PaymentType.HasValue && request.PaymentType.Value != order.PaymentType)
        {
            logger.LogDebug("PaymentType change detected for order {OrderId}: {OldPaymentType} -> {NewPaymentType}", request.OrderId, order.PaymentType, request.PaymentType.Value);
            changes[nameof(Order.PaymentType)] = (order.PaymentType, request.PaymentType.Value);
            order.PaymentType = request.PaymentType.Value;
        }

        if (changes.Count == 0)
        {
            logger.LogInformation("No changes detected for order {OrderId}", request.OrderId);
            return;
        }

        UpdateClient(order, request, changes);

        order.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordChangesAsync<Order>(
            entityId: order.Id,
            changes: changes,
            changedByUserId: request.CurrentUserId,
            changeDescription: "Order updated");

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully updated order {OrderId} with {ChangeCount} changes", request.OrderId, changes.Count);
    }

    private static void UpdateClient(
        Order order,
        UpdateOrderCommand request,
        Dictionary<string, (object? OldValue, object? NewValue)> changes)
    {
        var hasClientData = request.ClientPhoneNumber is not null
            || request.ClientEmail is not null
            || request.ClientUserId.HasValue;

        if (!hasClientData && order.Client is null)
            return;

        if (!hasClientData && order.Client is not null)
        {
            changes["Client"] = ("present", null);
            order.Client = null;
            return;
        }

        order.Client ??= new OrderClient();

        if (request.ClientPhoneNumber != order.Client.PhoneNumber)
        {
            changes["Client.PhoneNumber"] = (order.Client.PhoneNumber, request.ClientPhoneNumber);
            order.Client.PhoneNumber = request.ClientPhoneNumber;
        }

        if (request.ClientEmail != order.Client.Email)
        {
            changes["Client.Email"] = (order.Client.Email, request.ClientEmail);
            order.Client.Email = request.ClientEmail;
        }

        if (request.ClientUserId != order.Client.UserId)
        {
            changes["Client.UserId"] = (order.Client.UserId, request.ClientUserId);
            order.Client.UserId = request.ClientUserId;
        }
    }
}
