









import { useState } from 'react';
import ReservationSearch from './ReservationSearch';
import GuestIdentityForm, { type IdentityFormValues } from './GuestIdentityForm';
import RoomSelector from './RoomSelector';
import { checkIn } from '../../api/receptionApi';
import type { ReservationSummaryDto, PaymentMethod, CheckInResultDto } from '../../types';

type Step = 'search' | 'identity' | 'room' | 'done';

const PAYMENT_METHODS: { value: PaymentMethod; icon: string; label: string }[] = [
  { value: 'Cash',       icon: '💵', label: 'Cash' },
  { value: 'CreditCard', icon: '💳', label: 'Credit Card' },
  { value: 'RoomBill',   icon: '🏨', label: 'Room Bill' },
];

interface Props {
  onDone?: () => void;
}

export default function CheckInForm({ onDone }: Props) {
  const [step, setStep] = useState<Step>('search');
  const [reservation, setReservation] = useState<ReservationSummaryDto | null>(null);
  const [identityValues, setIdentityValues] = useState<IdentityFormValues | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('CreditCard');
  const [alternativeRoomId, setAlternativeRoomId] = useState('');
  const [needsAlternativeRoom, setNeedsAlternativeRoom] = useState(false);
  const [result, setResult] = useState<CheckInResultDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function reset() {
    setStep('search'); setReservation(null); setIdentityValues(null);
    setAlternativeRoomId(''); setNeedsAlternativeRoom(false);
    setResult(null); setLoading(false); setError(null);
  }

  async function submitCheckIn(identity: IdentityFormValues, altRoomId?: string) {
    if (!reservation) return;
    setLoading(true); setError(null);
    try {
      const checkInResult = await checkIn({
        reservationId: reservation.id,
        idType: identity.idType,
        idNumber: identity.idNumber,
        idExpiry: identity.idExpiry,
        paymentMethod,
        alternativeRoomId: altRoomId || undefined,
      });
      setResult(checkInResult);
      setStep('done');
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { error?: string } } };
      const message = axiosError?.response?.data?.error
        ?? (err instanceof Error ? err.message : 'Check-in failed');
      
      
      if (message.toLowerCase().includes('unavailable') || message.toLowerCase().includes('not available')) {
        setNeedsAlternativeRoom(true);
        setStep('room');
      } else {
        setError(message);
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="wizard-container">
      {step !== 'done' && step !== 'search' && (
        <button className="step-back-btn" onClick={() => setStep(step === 'identity' ? 'search' : 'identity')}>
          ← Back
        </button>
      )}

      {step === 'search' && (
        <ReservationSearch
          onSelect={(res) => { setReservation(res); setStep('identity'); }}
        />
      )}

      {step === 'identity' && reservation && (
        <div className="step-content">
          <div className="info-card" style={{ marginBottom: 20 }}>
            <div className="info-row">
              <span className="info-label">Guest</span>
              <span className="info-value">{reservation.guestName}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Room</span>
              <span className="info-value room-number-lg">{reservation.roomNumber}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Check-out</span>
              <span className="info-value">{new Date(reservation.checkOutDate).toLocaleDateString()}</span>
            </div>
          </div>

          <p className="section-subtitle">Payment method</p>
          <div className="payment-method-grid" style={{ marginBottom: 20 }}>
            {PAYMENT_METHODS.map((method) => (
              <label key={method.value} className={`payment-method-option${paymentMethod === method.value ? ' selected' : ''}`}>
                <input type="radio" name="payment" value={method.value} checked={paymentMethod === method.value} onChange={() => setPaymentMethod(method.value)} />
                <span className="pm-icon">{method.icon}</span>
                <span className="pm-label">{method.label}</span>
              </label>
            ))}
          </div>

          <GuestIdentityForm
            onSubmit={(values) => { setIdentityValues(values); submitCheckIn(values); }}
            onCancel={() => setStep('search')}
            loading={loading}
            error={error}
          />
        </div>
      )}

      {step === 'room' && needsAlternativeRoom && identityValues && (
        <div className="step-content">
          <h2 className="step-title">Select Alternative Room</h2>
          <p className="body-text">The reserved room is unavailable. Please select an alternative.</p>
          <RoomSelector
            selectedRoomId={alternativeRoomId}
            onChange={setAlternativeRoomId}
          />
          {error && <div className="alert alert-error"><span className="alert-icon">⚠</span><span>{error}</span></div>}
          <div className="btn-row">
            <button
              className="btn btn-primary"
              disabled={!alternativeRoomId || loading}
              onClick={() => submitCheckIn(identityValues, alternativeRoomId)}
            >
              {loading ? 'Processing…' : 'Confirm check-in'}
            </button>
          </div>
        </div>
      )}

      {step === 'done' && result && (
        <div className="step-content confirmation-view">
          <div className="confirmation-icon">✓</div>
          <h2 className="step-title confirmation-title">Check-In Complete</h2>
          <div className="key-display">
            <span className="key-icon">🔑</span>
            <div className="key-info">
              <span className="key-label">Room key</span>
              <span className="key-number">{result.keyNumber}</span>
            </div>
          </div>
          <div className="info-card confirmation-card">
            <div className="info-row">
              <span className="info-label">Room number</span>
              <span className="info-value room-number-lg">{result.roomNumber}</span>
            </div>
          </div>
          <div className="btn-row" style={{ justifyContent: 'center' }}>
            <button className="btn btn-primary btn-large" onClick={reset}>New check-in</button>
            {onDone && <button className="btn btn-ghost btn-large" onClick={onDone}>Dashboard</button>}
          </div>
        </div>
      )}
    </div>
  );
}
