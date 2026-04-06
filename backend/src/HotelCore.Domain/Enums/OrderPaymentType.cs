namespace HotelCore.Domain.Enums;

/// <summary>
/// Defines the payment method for an order.
/// </summary>
public enum OrderPaymentType
{
    /// <summary>
    /// Payment is made in advance before work starts.
    /// </summary>
    Prepay = 1,
    
    /// <summary>
    /// Payment is held in escrow and released upon completion.
    /// </summary>
    Escrow = 2
}
