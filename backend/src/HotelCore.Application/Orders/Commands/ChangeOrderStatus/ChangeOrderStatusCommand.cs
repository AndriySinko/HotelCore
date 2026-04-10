// This file contains code for ChangeOrderStatusCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Commands.ChangeOrderStatus;

public record ChangeOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    Guid CurrentUserId
) : IRequest;
