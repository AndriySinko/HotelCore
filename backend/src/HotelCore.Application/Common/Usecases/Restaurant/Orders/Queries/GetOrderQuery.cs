using MediatR;
using HotelCore.Application.Common.DTOs.Orders;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<OrderResponse>;
