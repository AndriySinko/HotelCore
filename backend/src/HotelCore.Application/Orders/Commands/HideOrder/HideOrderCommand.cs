using MediatR;

namespace HotelCore.Application.Orders.Commands.HideOrder;

public record HideOrderCommand(Guid OrderId, Guid CurrentUserId) : IRequest;
