using System;
using System.Collections.Generic;
using System.Text;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.DTOs;

public record MyImageGroupDto(
    Guid Id,
    string? Title,
    int Position,
    IReadOnlyList<MyImageDto> Images);
