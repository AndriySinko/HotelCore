namespace HotelCore.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an order.
/// </summary>
public enum OrderStatus
{
    Draft = 1,
    Active = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}
