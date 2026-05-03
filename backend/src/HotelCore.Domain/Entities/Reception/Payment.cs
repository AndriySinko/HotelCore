// This file contains code for Payment.
using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Reception;







public class Payment : BaseEntity
{
    public Guid ReservationId { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime TransactionDate { get; init; }
    public string? ReferenceNumber { get; set; }

    
    public Reservation? Reservation { get; set; }

    public void SetStatus(PaymentStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
