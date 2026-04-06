using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Orders;

public class Order : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public OrderPaymentType PaymentType { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public bool IsHidden { get; set; }
    public OrderType Type { get; set; } = OrderType.Regular;

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public OrderClient? Client { get; set; }

    public Guid? ParentOrderId { get; set; }
    public Order? ParentOrder { get; set; }

    public ICollection<Order> SubOrders { get; init; } = [];

    public ICollection<MyImageGroup> Images { get; init; } = [];
}
