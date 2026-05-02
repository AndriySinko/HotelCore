import { useState, useEffect, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { QRCodeSVG } from 'qrcode.react';
import { getAvailableRooms, createReservation } from '../api/receptionApi';
import type { RoomDto, CreateReservationResultDto, RoomType } from '../types';
import Alert from '../components/Alert';

const today    = new Date().toISOString().split('T')[0];
const nextWeek = new Date(Date.now() + 7 * 86400000).toISOString().split('T')[0];

const ROOM_TYPES: { value: RoomType; label: string; description: string; price: number; capacity: number }[] = [
  { value: 'Single', label: 'Single',  description: 'Cozy room for one guest',           price: 80,  capacity: 1 },
  { value: 'Double', label: 'Double',  description: 'Comfortable room for two guests',   price: 120, capacity: 2 },
  { value: 'Family', label: 'Family',  description: 'Spacious room for up to 5 guests',  price: 160, capacity: 5 },
  { value: 'Deluxe', label: 'Deluxe',  description: 'Premium room with city views',      price: 200, capacity: 2 },
  { value: 'Suite',  label: 'Suite',   description: 'Luxury suite with separate lounge', price: 350, capacity: 4 },
];

export default function BookReservationPage() {
  const [step, setStep] = useState<'form' | 'success'>('form');

  const [checkIn,  setCheckIn]  = useState(today);
  const [checkOut, setCheckOut] = useState(nextWeek);
  const [roomType, setRoomType] = useState<RoomType | ''>('');

  const [rooms,        setRooms]        = useState<RoomDto[]>([]);
  const [roomsLoading, setRoomsLoading] = useState(false);

  const [firstName, setFirstName] = useState('');
  const [lastName,  setLastName]  = useState('');
  const [email,     setEmail]     = useState('');
  const [phone,     setPhone]     = useState('');
  const [guests,    setGuests]    = useState(1);

  const [error,      setError]      = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [result,     setResult]     = useState<CreateReservationResultDto | null>(null);

  useEffect(() => {
    if (!checkIn || !checkOut || checkOut <= checkIn) return;
    setRoomsLoading(true);
    getAvailableRooms(checkIn, checkOut)
      .then(setRooms)
      .catch(() => setRooms([]))
      .finally(() => setRoomsLoading(false));
  }, [checkIn, checkOut]);

  const nights = checkIn && checkOut
    ? Math.max(0, Math.round((new Date(checkOut).getTime() - new Date(checkIn).getTime()) / 86400000))
    : 0;

  const datesValid = !!(checkIn && checkOut && checkOut > checkIn);

  const autoAssignedRoom = roomType
    ? rooms.find(r => r.roomType === roomType) ?? null
    : null;

  const selectedTypeInfo = ROOM_TYPES.find(t => t.value === roomType) ?? null;

  const isTypeAvailable = (type: RoomType) => rooms.some(r => r.roomType === type);
  const isTypeVisible   = (type: { capacity: number; value: RoomType }) =>
    type.capacity >= guests && (!datesValid || isTypeAvailable(type.value));

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!autoAssignedRoom) { setError('Please select an available room type.'); return; }
    setError(null);
    setSubmitting(true);
    try {
      const res = await createReservation({
        firstName, lastName, email, phone,
        roomId: autoAssignedRoom.id,
        checkInDate:  `${checkIn}T14:00:00`,
        checkOutDate: `${checkOut}T11:00:00`,
        numberOfGuests: guests,
      });
      setResult(res);
      setStep('success');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Something went wrong. Please try again.');
    } finally {
      setSubmitting(false);
    }
  }

  function reset() {
    setStep('form');
    setResult(null);
    setRoomType('');
    setFirstName(''); setLastName(''); setEmail(''); setPhone('');
    setGuests(1);
  }

  if (step === 'success' && result) {
    return (
      <div style={{ minHeight: '100vh', background: 'var(--c-bg)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '24px 16px' }}>
        <div className="wizard-container" style={{ width: '100%' }}>
          <div className="wizard-header">
            <div className="wizard-brand"><span className="brand-icon">🏨</span> HotelCore HMS</div>
            <h1 className="wizard-heading">Reservation Confirmed</h1>
          </div>
          <div className="wizard-body">
            <div className="step-content confirmation-view">
              <div className="confirmation-icon">✓</div>
              <h2 className="step-title confirmation-title">You're all set!</h2>
              <p className="body-text">Present your QR code at the reception desk on arrival.</p>

              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12, marginBottom: 8 }}>
                <div style={{ background: '#fff', padding: 12, borderRadius: 8, border: '1px solid var(--c-border)' }}>
                  <QRCodeSVG
                    value={`${window.location.origin}/reservation/${result.qrCode}`}
                    size={180}
                    level="M"
                  />
                </div>
                <div style={{ textAlign: 'center' }}>
                  <div style={{ fontSize: 12, color: 'var(--c-muted)', marginBottom: 2 }}>Confirmation Code</div>
                  <div style={{ fontSize: 24, fontWeight: 700, letterSpacing: '0.1em', color: 'var(--c-primary)' }}>{result.qrCode}</div>
                </div>
              </div>

              <div className="info-card confirmation-card">
                <div className="info-row">
                  <span className="info-label">Room</span>
                  <span className="info-value room-number-lg">{result.roomNumber}</span>
                </div>
                <div className="info-row">
                  <span className="info-label">Check-in</span>
                  <span className="info-value">{new Date(`${checkIn}T14:00:00`).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
                </div>
                <div className="info-row">
                  <span className="info-label">Check-out</span>
                  <span className="info-value">{new Date(`${checkOut}T11:00:00`).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
                </div>
                <div className="info-row">
                  <span className="info-label">Total</span>
                  <span className="info-value" style={{ fontWeight: 700, color: 'var(--c-primary)', fontSize: 16 }}>${result.totalPrice.toFixed(2)}</span>
                </div>
                <div className="info-row">
                  <span className="info-label">Confirmation sent to</span>
                  <span className="info-value">{email}</span>
                </div>
              </div>

              <div className="btn-row" style={{ marginTop: '1.5rem', gap: 12 }}>
                <button className="btn btn-secondary" onClick={reset}>Make another reservation</button>
                <Link to="/guest" className="btn btn-primary">Back to dashboard</Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', background: 'var(--c-bg)', padding: '24px 16px 48px' }}>
      <div className="wizard-container">
        <div className="wizard-header">
          <div className="wizard-brand"><span className="brand-icon">🏨</span> HotelCore HMS</div>
          <h1 className="wizard-heading">Book a Room</h1>
        </div>

        <div className="wizard-body">
          <div className="step-content">
            <h2 className="step-title">Guest Reservation</h2>
            <p className="body-text">Choose your dates, select a room type, and fill in your details.</p>

            {error && <Alert type="error" message={error} onDismiss={() => setError(null)} />}

            <form onSubmit={handleSubmit} className="form-grid">

              <div>
                <p className="section-subtitle">Stay Dates</p>
                <div className="form-row">
                  <div className="form-field">
                    <label htmlFor="checkIn">Check-in date</label>
                    <input id="checkIn" type="date" value={checkIn} min={today}
                      onChange={e => { setCheckIn(e.target.value); setRoomType(''); }} required />
                  </div>
                  <div className="form-field">
                    <label htmlFor="checkOut">Check-out date</label>
                    <input id="checkOut" type="date" value={checkOut} min={checkIn || today}
                      onChange={e => { setCheckOut(e.target.value); setRoomType(''); }} required />
                  </div>
                </div>

                {datesValid && (
                  <div className="form-field" style={{ marginTop: 12, maxWidth: 200 }}>
                    <label htmlFor="guests">Number of guests *</label>
                    <input id="guests" type="number" min={1} max={10}
                      value={guests} onChange={e => { setGuests(Number(e.target.value)); setRoomType(''); }} required />
                  </div>
                )}
              </div>

              {datesValid && (
                <div>
                  <p className="section-subtitle">Room Type</p>

                  {roomsLoading ? (
                    <div style={{ display: 'flex', alignItems: 'center', gap: 10, color: 'var(--c-muted)', fontSize: 14, padding: '8px 0' }}>
                      <div className="spinner" style={{ width: 20, height: 20, marginBottom: 0 }} />
                      Checking availability…
                    </div>
                  ) : (
                    <>
                      {ROOM_TYPES.filter(isTypeVisible).length === 0 && (
                        <p style={{ color: 'var(--c-muted)', fontSize: 14 }}>
                          No rooms available for {guests} guest{guests !== 1 ? 's' : ''} on these dates.
                        </p>
                      )}
                      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                        {ROOM_TYPES.filter(isTypeVisible).map(type => {
                          const selected = roomType === type.value;
                          return (
                            <label
                              key={type.value}
                              className={`room-option${selected ? ' selected' : ''}`}
                            >
                              <input
                                type="radio"
                                name="roomType"
                                value={type.value}
                                checked={selected}
                                onChange={() => setRoomType(type.value)}
                              />
                              <div className="room-option-body">
                                <span className="room-number">{type.label}</span>
                                <span className="room-type">{type.description}</span>
                                <span style={{ fontSize: 12, color: 'var(--c-muted)' }}>Up to {type.capacity} guest{type.capacity !== 1 ? 's' : ''}</span>
                              </div>
                              <span style={{ fontWeight: 700, color: 'var(--c-primary)', fontSize: 15, flexShrink: 0 }}>
                                ${type.price}/night{nights > 0 ? ` · $${(type.price * nights).toFixed(0)} total` : ''}
                              </span>
                            </label>
                          );
                        })}
                      </div>
                    </>
                  )}
                </div>
              )}

              <div>
                <p className="section-subtitle">Guest Details</p>
                <div className="form-grid">
                  <div className="form-row">
                    <div className="form-field">
                      <label htmlFor="firstName">First name *</label>
                      <input id="firstName" type="text" placeholder="John"
                        value={firstName} onChange={e => setFirstName(e.target.value)} required />
                    </div>
                    <div className="form-field">
                      <label htmlFor="lastName">Last name *</label>
                      <input id="lastName" type="text" placeholder="Doe"
                        value={lastName} onChange={e => setLastName(e.target.value)} required />
                    </div>
                  </div>

                  <div className="form-field">
                    <label htmlFor="email">Email address *</label>
                    <input id="email" type="email" placeholder="john.doe@example.com"
                      value={email} onChange={e => setEmail(e.target.value)} required />
                  </div>

                  <div className="form-field">
                    <label htmlFor="phone">Phone number *</label>
                    <input id="phone" type="tel" placeholder="+1 234 567 8900"
                      value={phone} onChange={e => setPhone(e.target.value)} required />
                  </div>
                </div>
              </div>

              {selectedTypeInfo && autoAssignedRoom && nights > 0 && (
                <div className="payment-summary-card">
                  <div className="payment-summary-header">
                    <span>Booking Summary</span>
                    <span>{nights} night{nights !== 1 ? 's' : ''}</span>
                  </div>
                  <div className="payment-line">
                    <span>{selectedTypeInfo.label} room — {selectedTypeInfo.description}</span>
                  </div>
                  <div className="payment-line">
                    <span>Rate per night</span>
                    <span>${autoAssignedRoom.pricePerNight.toFixed(2)}</span>
                  </div>
                  <div className="payment-total-row">
                    <span>Total</span>
                    <span className="payment-total-amount">${(autoAssignedRoom.pricePerNight * nights).toFixed(2)}</span>
                  </div>
                </div>
              )}

              <div className="btn-row">
                <button type="submit" className="btn btn-primary btn-large"
                  disabled={submitting || !autoAssignedRoom}>
                  {submitting ? 'Booking…' : 'Confirm Reservation'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
