using MediatR;
using HotelCore.Application.Common.DTOs.Orders;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;

public record CreateOrderItemDto(Guid ProductId, int Quantity, string? SpecialRequest);

public record CreateOrderCommand(
    List<CreateOrderItemDto> Items,
    PaymentMethod PaymentMethod) : IRequest<OrderResponse>;
