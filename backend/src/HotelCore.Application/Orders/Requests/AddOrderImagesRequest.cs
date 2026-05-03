// This file contains code for AddOrderImagesRequest.
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Orders.Requests
{
    public record AddOrderImagesRequest
    {
        public required IReadOnlyList<IFormFile> Images { get; set; }
    }
}
