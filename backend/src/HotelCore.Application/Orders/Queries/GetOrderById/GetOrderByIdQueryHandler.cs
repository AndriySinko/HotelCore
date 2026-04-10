// This file contains code for GetOrderByIdQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Orders.DTOs;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository, IOrderAccessPolicy accessPolicy) 
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new NotFoundException(nameof(Order), request.OrderId);

        var isAuthorizedParty = accessPolicy.IsAuthorizedParty(order, request.CurrentUserId, request.GuestToken);

        if ((order.IsHidden || order.Status == Domain.Enums.OrderStatus.Draft) && !isAuthorizedParty)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        return new OrderDto
        {
            Id = order.Id,
            Title = order.Title,
            Description = order.Description,
            Price = order.Price,
            PaymentType = order.PaymentType,
            Status = order.Status,
            Type = order.Type,
            IsHidden = order.IsHidden,
            CreatedByUserId = order.CreatedByUserId,
            CreatedByUserName = order.CreatedByUser?.UserName ?? string.Empty,
            Client = isAuthorizedParty && order.Client is not null
                ? new OrderClientDto
                {
                    PhoneNumber = order.Client.PhoneNumber,
                    Email = order.Client.Email,
                    UserId = order.Client.UserId,
                    UserName = order.Client.User?.UserName
                }
                : null,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
