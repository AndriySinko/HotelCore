using MediatR;
using HotelCore.Application.Orders.DTOs;

namespace HotelCore.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId, Guid? CurrentUserId, string? GuestToken = null) : IRequest<OrderDto>;
