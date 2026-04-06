using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.UnhideOrder;

public class UnhideOrderCommandHandler(
    IOrderRepository orderRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork,
    IOrderAccessPolicy accessPolicy,
    ILogger<UnhideOrderCommandHandler> logger) 
    : IRequestHandler<UnhideOrderCommand>
{
    public async Task Handle(UnhideOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unhiding order with OrderId: {OrderId}", request.OrderId);

        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (!accessPolicy.IsOwner(order, request.CurrentUserId))
        {
             throw new ForbiddenException("You do not have permission to perform this action");
        }

        if (!order.IsHidden)
        {
            logger.LogInformation("Order is already unhidde with OrderId: {OrderId}", request.OrderId);
            return;
        }

        order.IsHidden = false;
        order.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordPropertyChangeAsync<Order>(
            entityId: order.Id,
            propertyName: nameof(Order.IsHidden),
            oldValue: true,
            newValue: false,
            changedByUserId: request.CurrentUserId,
            changeDescription: "Order unhidden" // TODO: change to dynamic language translations
        );

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order with OrderId: {OrderId} has been unhidden", request.OrderId);
    }
}
