// This file contains code for UnhideOrderCommand.
using MediatR;

namespace HotelCore.Application.Orders.Commands.UnhideOrder;

public record UnhideOrderCommand(Guid OrderId, Guid CurrentUserId) : IRequest;
