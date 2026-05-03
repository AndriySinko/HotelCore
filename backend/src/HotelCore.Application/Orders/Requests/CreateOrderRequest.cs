// This file contains code for CreateOrderRequest.
using System;
using System.Collections.Generic;
using System.Text;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Requests;

public record CreateOrderRequest(
    string Title,
    string? Description = null,
    decimal? Price = null,
    OrderPaymentType? PaymentType = null,
    string? ClientPhoneNumber = null,
    string? ClientEmail = null,
    Guid? ClientUserId = null);
