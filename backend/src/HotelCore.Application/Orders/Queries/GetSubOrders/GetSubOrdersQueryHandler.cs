// This file contains code for GetSubOrdersQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Orders.DTOs;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Queries.GetSubOrders;

public class GetSubOrdersQueryHandler(IOrderRepository orderRepository, IOrderAccessPolicy accessPolicy)
    : IRequestHandler<GetSubOrdersQuery, IReadOnlyList<SimpleOrderDto>>
{
    public async Task<IReadOnlyList<SimpleOrderDto>> Handle(
        GetSubOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var parent = await orderRepository.GetByIdAsync(request.ParentOrderId, cancellationToken);

        if (parent is null)
            throw new NotFoundException(nameof(Order), request.ParentOrderId);

        var isAuthorizedParty = accessPolicy.IsAuthorizedParty(parent, request.CurrentUserId, request.GuestToken);

        if ((parent.IsHidden || parent.Status == OrderStatus.Draft) && !isAuthorizedParty)
            throw new NotFoundException(nameof(Order), request.ParentOrderId);

        var subOrders = await orderRepository.GetSubOrdersAsync(request.ParentOrderId, cancellationToken);

        return subOrders
            .Where(o => isAuthorizedParty || (!o.IsHidden && o.Status != OrderStatus.Draft))
            .Select(o => new SimpleOrderDto
            {
                Id = o.Id,
                Title = o.Title,
                Status = o.Status,
                Type = o.Type,
                Price = o.Price,
                IsHidden = o.IsHidden,
                CreatedAt = o.CreatedAt
            })
            .ToList();
    }
}
