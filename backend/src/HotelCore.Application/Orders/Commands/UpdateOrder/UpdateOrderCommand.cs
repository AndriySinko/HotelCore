using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(
    Guid OrderId,
    Guid CurrentUserId,
    string? Title,
    string? Description,
    decimal? Price,
    OrderPaymentType? PaymentType,
    string? ClientPhoneNumber,
    string? ClientEmail,
    Guid? ClientUserId) : IRequest;
