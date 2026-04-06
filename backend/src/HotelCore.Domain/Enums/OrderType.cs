namespace HotelCore.Domain.Enums;

/// <summary>
/// Distinguishes top-level orders from sub-orders scoped to a parent.
/// </summary>
public enum OrderType
{
    Regular = 1,
    Sub = 2
}
