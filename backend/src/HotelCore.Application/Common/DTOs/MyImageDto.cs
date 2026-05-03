// This file contains code for MyImageDto.
using System;
using System.Collections.Generic;
using System.Text;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.DTOs;

public record MyImageDto(
    Guid Id,
    string Url,
    int Width,
    int Height,
    long SizeBytes,
    double AspectRatio);
