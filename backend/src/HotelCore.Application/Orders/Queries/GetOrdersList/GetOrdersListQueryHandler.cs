using MediatR;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Orders.DTOs;

namespace HotelCore.Application.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryHandler(IOrderRepository orderRepository) 
    : IRequestHandler<GetOrdersListQuery, IReadOnlyList<SimpleOrderDto>>
{
    public async Task<IReadOnlyList<SimpleOrderDto>> Handle(
        GetOrdersListQuery request, 
        CancellationToken cancellationToken)
    {
        var isOwnerRequest = request.CurrentUserId.HasValue 
            && request.Filter.CreatedByUserId == request.CurrentUserId;

        var filter = request.Filter with { Type = Domain.Enums.OrderType.Regular };
        if (!isOwnerRequest)
        {
            filter = filter with { 
                IsHidden = false, 
                ExcludeStatus = Domain.Enums.OrderStatus.Draft 
            };
        }

        var orders = await orderRepository.GetOrdersAsync(
            filter,
            request.Page,
            cancellationToken);

        var result = orders.Select(o => new SimpleOrderDto
        {
            Id = o.Id,
            Title = o.Title,
            Status = o.Status,
            Type = o.Type,
            Price = o.Price,
            IsHidden = o.IsHidden,
            CreatedAt = o.CreatedAt,
            ClientName = isOwnerRequest ? o.Client?.User?.UserName : null,
            ClientPhoneNumber = isOwnerRequest ? o.Client?.PhoneNumber : null,
            ClientEmail = isOwnerRequest ? o.Client?.Email : null
        }).ToList();

        return result;
    }
}
