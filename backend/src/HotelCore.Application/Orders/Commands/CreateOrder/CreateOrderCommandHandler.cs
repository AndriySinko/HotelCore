// This file contains code for CreateOrderCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Helpers;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new order with title: {Title} for user: {UserId}", request.Title, request.CreatedByUserId);

        var hasClient = request.ClientPhoneNumber is not null
            || request.ClientEmail is not null
            || request.ClientUserId.HasValue;

        var order = new Order
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            PaymentType = request.PaymentType ?? OrderPaymentType.Prepay,
            Status = OrderStatus.Draft,
            CreatedByUserId = request.CreatedByUserId,
            Client = hasClient
                ? new OrderClient
                {
                    PhoneNumber = request.ClientPhoneNumber,
                    Email = request.ClientEmail,
                    UserId = request.ClientUserId
                }
                : null
        };

        orderRepository.Add(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Order created with ID: {OrderId}", order.Id);

        return order.Id;
    }
}
