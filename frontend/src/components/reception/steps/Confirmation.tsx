import { useEffect, useState } from 'react';
import type { ReservationDto, RoomDto, IdentityData, PaymentMethod, CheckInCompleteResult } from '../../../types/checkIn';
import { completeCheckIn } from '../../../api/checkInApi';
import Alert from '../../Alert';

interface Props {
  reservation: ReservationDto;
  selectedRoom: RoomDto;
  identityData: IdentityData;
  paymentMethod: PaymentMethod;
  onNewCheckIn: () => void;
}

export default function Confirmation({ reservation, selectedRoom, identityData, paymentMethod, onNewCheckIn }: Props) {
  const [result, setResult]   = useState<CheckInCompleteResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    completeCheckIn(reservation.id, identityData, null, paymentMethod)
      .then(data => { if (!cancelled) setResult(data); })
      .catch(err  => { if (!cancelled) setError(err instanceof Error ? err.message : 'Check-in failed.'); })
      .finally(()  => { if (!cancelled) setLoading(false); });
    return () => { cancelled = true; };
  
  }, []);

  if (loading) {
    return (
      <div className="step-content centered">
        <div className="spinner" />
        <p className="body-text">Finalising check-in…</p>
      </div>
    );
  }

  if (error || !result) {
    return (
      <div className="step-content">
        <h2 className="step-title">Check-In Failed</h2>
        <Alert type="error" message={error ?? 'An unexpected error occurred.'} />
        <div className="btn-row">
          <button className="btn btn-primary" onClick={onNewCheckIn}>Start New Check-In</button>
        </div>
      </div>
    );
  }

  const roomNumber = result.roomNumber || selectedRoom.number;
  const guestName  = `${reservation.guest.firstName} ${reservation.guest.lastName}`;

  return (
    <div className="step-content confirmation-view">
      <div className="confirmation-icon">✓</div>
      <h2 className="step-title confirmation-title">Check-In Complete</h2>

      <div className="key-display">
        <div className="key-icon">🗝️</div>
        <div className="key-info">
          <span className="key-label">Key Number</span>
          <span className="key-number">{result.keyNumber}</span>
        </div>
      </div>

      <div className="info-card confirmation-card">
        <div className="info-row">
          <span className="info-label">Guest</span>
          <span className="info-value">{guestName}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Room</span>
          <span className="info-value room-number-lg">{roomNumber}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Status</span>
          <span className="info-value"><span className="badge badge-checkedin">Checked In</span></span>
        </div>
        <div className="info-row">
          <span className="info-label">Check-In</span>
          <span className="info-value">{new Date(reservation.checkInDate).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Check-Out</span>
          <span className="info-value">{new Date(reservation.checkOutDate).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
        </div>
      </div>

      <div className="btn-row" style={{ marginTop: '1.5rem' }}>
        <button className="btn btn-primary" onClick={onNewCheckIn}>New Check-In</button>
      </div>
    </div>
  );
}
