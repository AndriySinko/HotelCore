using MediatR;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork,
    IOrderAccessPolicy accessPolicy) 
    : IRequestHandler<ChangeOrderStatusCommand>
{
    public async Task Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (!accessPolicy.IsOwner(order, request.CurrentUserId))
        {
             throw new ForbiddenException("You do not have permission to perform this action");
        }

        if (order.Status == request.NewStatus)
        {
            return;
        }

        ValidateStatusTransition(order.Status, request.NewStatus);

        var oldStatus = order.Status;
        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordPropertyChangeAsync<Order>(
            entityId: order.Id,
            propertyName: nameof(Order.Status),
            oldValue: oldStatus,
            newValue: request.NewStatus,
            changedByUserId: request.CurrentUserId,
            changeDescription: $"Order status changed from {oldStatus} to {request.NewStatus}" // TODO: change to dynamic language translations
        );

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        var invalidTransitions = new Dictionary<OrderStatus, OrderStatus[]>
        {
            [OrderStatus.Completed] = new[] { OrderStatus.Draft, OrderStatus.Active, OrderStatus.InProgress },
            [OrderStatus.Cancelled] = new[] { OrderStatus.Draft, OrderStatus.Active, OrderStatus.InProgress }
        };

        if (invalidTransitions.TryGetValue(currentStatus, out var disallowedStatuses) 
            && disallowedStatuses.Contains(newStatus))
        {
            throw new InvalidOperationException($"Cannot transition from {currentStatus} to {newStatus}.");
        }
    }
}
