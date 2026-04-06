using MediatR;
using HotelCore.Application.Common.DTOs;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Common.Mappers;

using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Queries.GetOrderImages;

public class GetOrderImagesQueryHandler(IOrderRepository orderRepository, IOrderAccessPolicy accessPolicy) 
    : IRequestHandler<GetOrderImagesQuery, IReadOnlyList<MyImageGroupDto>>
{
    public async Task<IReadOnlyList<MyImageGroupDto>> Handle(
        GetOrderImagesQuery request, 
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if ((order.IsHidden || order.Status == Domain.Enums.OrderStatus.Draft)
            && !accessPolicy.IsAuthorizedParty(order, request.CurrentUserId, request.GuestToken))
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var imageGroups = await orderRepository.GetOrderImagesAsync(
            request.OrderId,
            request.Page,
            cancellationToken);

        return imageGroups.ToDto();
    }
}
