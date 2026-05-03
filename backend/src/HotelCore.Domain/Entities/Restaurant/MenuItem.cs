// This file contains code for MenuItem.
using HotelCore.Domain.Common;

namespace HotelCore.Domain.Entities.Restaurant;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
