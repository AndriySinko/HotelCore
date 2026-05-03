import { useState } from 'react';
import type { ReservationDto, RoomDto, PaymentMethod } from '../../../types/checkIn';
import Alert from '../../Alert';

interface Props {
  reservation: ReservationDto;
  selectedRoom: RoomDto;
  onPaymentComplete: (method: PaymentMethod) => void;
  onCancel: () => void;
}

const PAYMENT_METHODS: { value: PaymentMethod; label: string; icon: string }[] = [
  { value: 'Cash',       label: 'Cash',        icon: '💵' },
  { value: 'CreditCard', label: 'Credit Card',  icon: '💳' },
  { value: 'RoomBill',   label: 'Room Bill',    icon: '🏨' },
];

export default function Payment({ reservation, selectedRoom, onPaymentComplete, onCancel }: Props) {
  const [method, setMethod] = useState<PaymentMethod>('CreditCard');
  const [error, setError]   = useState<string | null>(null);

  const nights      = Math.max(1, (new Date(reservation.checkOutDate).getTime() - new Date(reservation.checkInDate).getTime()) / 86_400_000);
  const pricePerNight = selectedRoom.roomType.pricePerNight;
  const totalAmount   = Math.round(nights * pricePerNight * 100) / 100;

  function handlePay(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    onPaymentComplete(method);
  }

  return (
    <div className="step-content">
      <h2 className="step-title">Payment Summary</h2>
      <p className="body-text">Review the charges and select a payment method.</p>

      {error && <Alert type="error" message={error} onDismiss={() => setError(null)} />}

      <div className="payment-summary-card">
        <div className="payment-summary-header">
          <span>{reservation.confirmationNumber}</span>
          <span>{reservation.guest.firstName} {reservation.guest.lastName}</span>
        </div>
        <div className="payment-line">
          <span>Room {selectedRoom.number} — {selectedRoom.roomType.name}</span>
        </div>
        <div className="payment-line">
          <span>{new Date(reservation.checkInDate).toLocaleDateString()} → {new Date(reservation.checkOutDate).toLocaleDateString()}</span>
          <span>{nights} night{nights !== 1 ? 's' : ''}</span>
        </div>
        <div className="payment-line">
          <span>Rate per night</span>
          <span>${pricePerNight.toFixed(2)}</span>
        </div>
        <div className="payment-total-row">
          <span>Total</span>
          <span className="payment-total-amount">${totalAmount.toFixed(2)}</span>
        </div>
      </div>

      <form onSubmit={handlePay} className="form-grid" style={{ marginTop: '1.5rem' }}>
        <div className="form-field">
          <label>Payment Method *</label>
          <div className="payment-method-grid">
            {PAYMENT_METHODS.map(pm => (
              <label key={pm.value} className={`payment-method-option ${method === pm.value ? 'selected' : ''}`}>
                <input
                  type="radio"
                  name="payment-method"
                  value={pm.value}
                  checked={method === pm.value}
                  onChange={() => setMethod(pm.value)}
                />
                <span className="pm-icon">{pm.icon}</span>
                <span className="pm-label">{pm.label}</span>
              </label>
            ))}
          </div>
        </div>

        <div className="btn-row">
          <button type="submit" className="btn btn-primary btn-large">
            Pay ${totalAmount.toFixed(2)} & Check In
          </button>
          <button type="button" className="btn btn-ghost" onClick={onCancel}>Back</button>
        </div>
      </form>
    </div>
  );
}
