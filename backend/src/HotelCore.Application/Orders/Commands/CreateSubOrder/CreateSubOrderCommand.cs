// This file contains code for CreateSubOrderCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Commands.CreateSubOrder;

public record CreateSubOrderCommand(
    Guid ParentOrderId,
    string Title,
    string? Description,
    decimal? Price,
    OrderPaymentType? PaymentType,
    Guid CurrentUserId) : IRequest<Guid>;
