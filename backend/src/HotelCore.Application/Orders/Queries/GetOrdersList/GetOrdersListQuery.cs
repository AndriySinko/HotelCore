using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Orders.DTOs;
using HotelCore.Application.Orders.Models;

namespace HotelCore.Application.Orders.Queries.GetOrdersList;

public record GetOrdersListQuery(
    OrdersFilter Filter,
    PageRequest? Pagination = null,
    Guid? CurrentUserId = null
) : IRequest<IReadOnlyList<SimpleOrderDto>>
{
    public PageRequest Page => Pagination ?? PageRequest.Default;
}
