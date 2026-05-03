// This file contains code for UpdateOrderRequest.
using System;
using System.Collections.Generic;
using System.Text;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Requests;

public record UpdateOrderRequest(
    string? Title = null,
    string? Description = null,
    decimal? Price = null,
    OrderPaymentType? PaymentType = null,
    string? ClientPhoneNumber = null,
    string? ClientEmail = null,
    Guid? ClientUserId = null);
