// This file contains code for ChangeStatusRequest.
using System;
using System.Collections.Generic;
using System.Text;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Requests;

public record ChangeStatusRequest(OrderStatus NewStatus);
