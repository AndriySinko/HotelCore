namespace HotelCore.Application.Common.DTOs.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    string? ImageUrl,
    Guid CategoryId,
    string CategoryName);
