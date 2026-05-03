






import { useState } from 'react';
import ReservationSearch from './ReservationSearch';
import { checkOut } from '../../api/receptionApi';
import type { ReservationSummaryDto, CheckOutResultDto } from '../../types';

type Step = 'search' | 'confirm' | 'done';

interface Props {
  onDone?: () => void;
}

export default function CheckOutForm({ onDone }: Props) {
  const [step, setStep] = useState<Step>('search');
  const [reservation, setReservation] = useState<ReservationSummaryDto | null>(null);
  const [result, setResult] = useState<CheckOutResultDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function reset() {
    setStep('search'); setReservation(null); setResult(null);
    setLoading(false); setError(null);
  }

  async function handleCheckOut() {
    if (!reservation) return;
    setLoading(true); setError(null);
    try {
      const checkOutResult = await checkOut(reservation.id);
      setResult(checkOutResult);
      setStep('done');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Check-out failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="wizard-container">
      {step === 'confirm' && (
        <button className="step-back-btn" onClick={() => setStep('search')}>← Back</button>
      )}

      {step === 'search' && (
        <ReservationSearch
          onSelect={(res) => {
            setReservation(res);
            setStep('confirm');
          }}
        />
      )}

      {step === 'confirm' && reservation && (
        <div className="step-content">
          <h2 className="step-title">Confirm Check-Out</h2>
          <p className="body-text">Review the reservation details before processing check-out.</p>

          <div className="info-card">
            <div className="info-row">
              <span className="info-label">Guest</span>
              <span className="info-value">{reservation.guestName}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Room</span>
              <span className="info-value room-number-lg">{reservation.roomNumber}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Check-in date</span>
              <span className="info-value">{new Date(reservation.checkInDate).toLocaleDateString()}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Check-out date</span>
              <span className="info-value">{new Date(reservation.checkOutDate).toLocaleDateString()}</span>
            </div>
          </div>

          {error && (
            <div className="alert alert-error">
              <span className="alert-icon">⚠</span>
              <span className="alert-message">{error}</span>
            </div>
          )}

          <div className="btn-row">
            <button className="btn btn-primary btn-large" disabled={loading} onClick={handleCheckOut}>
              {loading ? 'Processing…' : 'Confirm check-out'}
            </button>
            <button className="btn btn-ghost btn-large" onClick={() => setStep('search')}>Cancel</button>
          </div>
        </div>
      )}

      {step === 'done' && result && (
        <div className="step-content confirmation-view">
          <div className="confirmation-icon">✓</div>
          <h2 className="step-title confirmation-title">Check-Out Complete</h2>

          <div className="payment-summary-card confirmation-card" style={{ marginTop: 20 }}>
            <div className="payment-summary-header">
              <span>Guest</span>
              <span>{result.guestName}</span>
            </div>
            <div className="payment-line">
              <span>Room</span>
              <span className="room-number-lg">{result.roomNumber}</span>
            </div>
            <div className="payment-total-row">
              <span>Total charged</span>
              <span className="payment-total-amount">€{result.totalCharged.toFixed(2)}</span>
            </div>
          </div>

          <div className="btn-row" style={{ justifyContent: 'center' }}>
            <button className="btn btn-primary btn-large" onClick={reset}>New check-out</button>
            {onDone && <button className="btn btn-ghost btn-large" onClick={onDone}>Dashboard</button>}
          </div>
        </div>
      )}
    </div>
  );
}
