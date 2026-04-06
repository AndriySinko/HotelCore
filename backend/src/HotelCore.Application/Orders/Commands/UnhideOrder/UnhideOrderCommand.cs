using MediatR;

namespace HotelCore.Application.Orders.Commands.UnhideOrder;

public record UnhideOrderCommand(Guid OrderId, Guid CurrentUserId) : IRequest;
