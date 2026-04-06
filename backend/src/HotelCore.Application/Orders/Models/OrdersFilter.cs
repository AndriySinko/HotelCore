using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.Models;

public record OrdersFilter(
    Guid? CreatedByUserId = null,
    Guid? ClientUserId = null,
    string? GuestAccessToken = null,
    OrderStatus? Status = null,
    OrderStatus? ExcludeStatus = null,
    bool? IsHidden = null,
    OrderType? Type = null,
    OrderSortBy SortBy = OrderSortBy.CreatedAt,
    bool SortDescending = true);
