// This file contains code for MoveToAccountRequest.
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Orders.Requests;

public record MoveToAccountRequest(string GuestAccessToken);
