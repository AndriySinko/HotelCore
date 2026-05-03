// This file contains code for GetOrderImagesQuery.
using MediatR;
using HotelCore.Application.Common.DTOs;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Orders.Queries.GetOrderImages;

public record GetOrderImagesQuery(
    Guid OrderId,
    PageRequest? Pagination = null,
    Guid? CurrentUserId = null,
    string? GuestToken = null
) : IRequest<IReadOnlyList<MyImageGroupDto>>
{
    public PageRequest Page => Pagination ?? PageRequest.Default;
};
