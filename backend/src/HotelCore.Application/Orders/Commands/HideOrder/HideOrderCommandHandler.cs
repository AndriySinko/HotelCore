// This file contains code for HideOrderCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.HideOrder;

public class HideOrderCommandHandler(
    IOrderRepository orderRepository,
    IEntityHistoryService historyService,
    IUnitOfWork unitOfWork,
    IOrderAccessPolicy accessPolicy,
    ILogger<HideOrderCommandHandler> logger) 
    : IRequestHandler<HideOrderCommand>
{
    public async Task Handle(HideOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Hidding order with OrderId: {OrderId}", request.OrderId);
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (!accessPolicy.IsOwner(order, request.CurrentUserId))
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }

        if (order.IsHidden)
        {
            logger.LogInformation("Order with OrderId: {OrderId} is already hidden", request.OrderId);
            return;
        }

        order.IsHidden = true;
        order.UpdatedAt = DateTime.UtcNow;

        await historyService.RecordPropertyChangeAsync<Order>(
            entityId: order.Id,
            propertyName: nameof(Order.IsHidden),
            oldValue: false,
            newValue: true,
            changedByUserId: request.CurrentUserId,
            changeDescription: "Order hidden" 
        );

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order with OrderId: {OrderId} has been hidden successfully", request.OrderId);
    }
}
