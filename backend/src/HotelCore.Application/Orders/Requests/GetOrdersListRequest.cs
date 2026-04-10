// This file contains code for GetOrdersListRequest.
using HotelCore.Application.Common.Models;
using HotelCore.Application.Orders.Models;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Requests;

public sealed record GetOrdersListRequest : PageRequest
{
    public Guid? CreatedByUserId { get; init; }
    public Guid? ClientUserId { get; init; }
    public string? GuestAccessToken { get; init; }
    public OrderStatus? Status { get; init; }
    public bool? IsHidden { get; init; }
    public OrderSortBy SortBy { get; init; } = OrderSortBy.CreatedAt;
    public bool SortDescending { get; init; } = true;
}
