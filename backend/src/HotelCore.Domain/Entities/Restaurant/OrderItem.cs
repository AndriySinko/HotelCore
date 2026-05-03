// This file contains code for OrderItem.
using HotelCore.Domain.Common;

namespace HotelCore.Domain.Entities.Restaurant;

public class OrderItem : BaseEntity
{
    public Guid FoodOrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialRequest { get; set; }

    public FoodOrder? FoodOrder { get; set; }
    public MenuItem? MenuItem { get; set; }

    public decimal GetSubtotal() => Quantity * UnitPrice;
}
