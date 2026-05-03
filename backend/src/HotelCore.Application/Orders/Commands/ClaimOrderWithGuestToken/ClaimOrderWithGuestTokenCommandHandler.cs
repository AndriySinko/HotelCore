// This file contains code for ClaimOrderWithGuestTokenCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.ClaimOrderWithGuestToken;

public class ClaimOrderWithGuestTokenCommandHandler(
    IOrderRepository orderRepository,
    IOrdersTemporaryAccessStore temporaryAccessStore,
    IUnitOfWork unitOfWork,
    ILogger<ClaimOrderWithGuestTokenCommandHandler> logger)
    : IRequestHandler<ClaimOrderWithGuestTokenCommand>
{
    public async Task Handle(ClaimOrderWithGuestTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling ClaimOrderWithGuestTokenCommand for AccessToken: {AccessToken}", request.AccessToken);

        var orderId = await temporaryAccessStore.GetOrderIdAsync(request.AccessToken, cancellationToken)
            ?? throw new InvalidOperationException("Invalid or expired access token");

        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), orderId);

        if (order.Client is null)
        {
            logger.LogError("Order with ID {OrderId} has no client information", orderId);
            throw new InvalidOperationException("Order has no client information");
        }

        if (!string.IsNullOrEmpty(order.Client.GuestAccessToken))
        {
            logger.LogWarning("Order with ID {OrderId} has already been claimed with a guest access token", orderId);
            throw new InvalidOperationException("Order has already been claimed");
        }

        if (order.Client.UserId.HasValue)
        {
            logger.LogWarning("Order with ID {OrderId} is already linked to a registered user with ID {UserId}", orderId, order.Client.UserId);
            throw new InvalidOperationException("Order is already linked to a registered user");
        }

        order.Client.GuestAccessToken = request.GuestAccessToken;
        order.UpdatedAt = DateTime.UtcNow;

        orderRepository.Update(order);

        await temporaryAccessStore.DeleteAccessTokenAsync(request.AccessToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully claimed order with ID {OrderId} using guest access token", orderId);
    }
}
