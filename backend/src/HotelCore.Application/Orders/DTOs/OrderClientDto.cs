// This file contains code for OrderClientDto.
namespace HotelCore.Application.Orders.DTOs;

public class OrderClientDto
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public Guid? UserId { get; set; }
}
