// This file contains code for GetSubOrdersQuery.
using MediatR;
using HotelCore.Application.Orders.DTOs;

namespace HotelCore.Application.Orders.Queries.GetSubOrders;

public record GetSubOrdersQuery(
    Guid ParentOrderId,
    Guid? CurrentUserId,
    string? GuestToken = null) : IRequest<IReadOnlyList<SimpleOrderDto>>;
