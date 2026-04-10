// This file contains code for HideOrderCommand.
using MediatR;

namespace HotelCore.Application.Orders.Commands.HideOrder;

public record HideOrderCommand(Guid OrderId, Guid CurrentUserId) : IRequest;
