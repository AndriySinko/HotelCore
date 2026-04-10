// This file contains code for CreateSubOrderRequest.
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Requests;

public record CreateSubOrderRequest(
    string Title,
    string? Description = null,
    decimal? Price = null,
    OrderPaymentType? PaymentType = null);
