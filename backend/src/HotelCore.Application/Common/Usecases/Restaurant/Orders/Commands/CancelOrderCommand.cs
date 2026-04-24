using MediatR;
using HotelCore.Application.Common.DTOs.Orders;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<OrderResponse>;
