// This file contains code for FoodOrder.
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Restaurant;

public class FoodOrder : BaseEntity
{
    public Guid ReservationId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public FoodOrderStatus Status { get; private set; } = FoodOrderStatus.Placed;
    public string DeliveryTarget { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }
    public decimal TotalAmount { get; private set; }
    public DateTime? EstimatedDeliveryTime { get; set; }

    public Reservation? Reservation { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public void SetStatus(FoodOrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal CalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.GetSubtotal());
        return TotalAmount;
    }
}
