namespace HotelCore.Application.Common.DTOs.Orders;

public record OrderItemResponse(
    Guid Id,
    string ProductName,
    decimal PricePerUnit,
    int Quantity,
    string? SpecialRequest);

public record OrderResponse(
    Guid Id,
    string Status,
    List<OrderItemResponse> Items,
    decimal TotalPrice,
    string PaymentMethod,
    DateTime CreatedAt);
