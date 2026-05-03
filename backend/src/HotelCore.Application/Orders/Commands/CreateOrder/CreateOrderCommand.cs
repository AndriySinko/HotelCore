// This file contains code for CreateOrderCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    string Title,
    string? Description,
    decimal? Price,
    OrderPaymentType? PaymentType,
    string? ClientPhoneNumber,
    string? ClientEmail,
    Guid? ClientUserId,
    Guid CreatedByUserId) : IRequest<Guid>;
