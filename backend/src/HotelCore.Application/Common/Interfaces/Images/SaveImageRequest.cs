using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Common.Interfaces.Images;

public record SaveImageRequest(IFormFile File, int Position);