// payment record attached to a reservation — created at check-in
// Amount, Method and ReservationId use init so they cant be changed after the payment is recorded
using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Reception;

public class Payment : BaseEntity
{
    public Guid ReservationId { get; init; }
    // total amount charged for the stay — calculated from room rate * nights in CheckInCommandHandler
    public decimal Amount { get; init; }
    // cash, card, online — selected by the receptionist at check-in
    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime TransactionDate { get; init; }
    // auto-generated reference number like REF-A1B2C3D4 — can be used for receipts
    public string? ReferenceNumber { get; set; }

    public Reservation? Reservation { get; set; }

    public void SetStatus(PaymentStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
