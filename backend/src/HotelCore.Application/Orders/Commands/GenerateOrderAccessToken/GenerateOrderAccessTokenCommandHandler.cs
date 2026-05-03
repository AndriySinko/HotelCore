// This file contains code for GenerateOrderAccessTokenCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.GenerateOrderAccessToken;

public class GenerateOrderAccessTokenCommandHandler(
    IOrderRepository orderRepository,
    IOrdersTemporaryAccessStore temporaryAccessStore,
    TimeProvider timeProvider,
    ILogger<GenerateOrderAccessTokenCommandHandler> logger)
    : IRequestHandler<GenerateOrderAccessTokenCommand, GenerateOrderAccessTokenResult>
{
    private static readonly TimeSpan TokenExpiration = TimeSpan.FromDays(1);

    public async Task<GenerateOrderAccessTokenResult> Handle(
        GenerateOrderAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating access token for order {OrderId}", request.OrderId);
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var accessToken = await temporaryAccessStore.CreateAccessTokenAsync(
            request.OrderId,
            TokenExpiration,
            cancellationToken);

        var expiresAt = timeProvider.GetUtcNow().Add(TokenExpiration);
        logger.LogInformation("Generated access token for order {OrderId} with expiration at {ExpiresAt}", request.OrderId, expiresAt);

        return new GenerateOrderAccessTokenResult(accessToken, expiresAt);
    }
}
