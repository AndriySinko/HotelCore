using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public OrderPaymentType PaymentType { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType Type { get; set; }
    public bool IsHidden { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public OrderClientDto? Client { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
