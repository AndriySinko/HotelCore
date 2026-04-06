using HotelCore.Domain.Enums;

namespace HotelCore.Application.Orders.DTOs;

public class SimpleOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public OrderType Type { get; set; }
    public decimal? Price { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ClientName { get; set; }
    public string? ClientPhoneNumber { get; set; }
    public string? ClientEmail { get; set; }
}
