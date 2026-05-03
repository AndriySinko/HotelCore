// This file contains code for ClaimOrderRequest.
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Orders.Requests;

public record ClaimOrderRequest(string AccessToken, string GuestAccessToken);
