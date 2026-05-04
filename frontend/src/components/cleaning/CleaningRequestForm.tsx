import { useState, useEffect } from 'react';
import { requestCleaning } from '../../api/cleaningApi';
import { searchReservationsPublic, type PublicReservationDto } from '../../api/receptionApi';
import useAuthStore from '../../stores/authStore';
import type { CleaningRequestType } from '../../types';

const REQUEST_TYPES: { value: CleaningRequestType; label: string; desc: string }[] = [
  { value: 'GuestRequest', label: 'Standard cleaning',   desc: 'Regular tidying, fresh towels and linens' },
  { value: 'DeepClean',    label: 'Deep clean',          desc: 'Thorough cleaning of all surfaces and fixtures' },
  { value: 'Emergency',    label: 'Emergency / spill',   desc: 'Urgent issue that needs immediate attention' },
  { value: 'Turnover',     label: 'Checkout clean',      desc: 'Full room reset for your checkout day' },
];

const TIME_SLOTS: { label: string; time: string }[] = [
  { label: 'Morning   8:00–11:00',   time: '08:00' },
  { label: 'Midday   11:00–14:00',   time: '11:00' },
  { label: 'Afternoon  14:00–17:00', time: '14:00' },
  { label: 'Evening   17:00–20:00',  time: '17:00' },
];

const PRIORITIES: { value: number; label: string; color: string }[] = [
  { value: 1, label: 'Urgent',   color: '#dc2626' },
  { value: 2, label: 'High',     color: '#d97706' },
  { value: 3, label: 'Normal',   color: '#2563eb' },
  { value: 4, label: 'Low',      color: '#16a34a' },
  { value: 5, label: 'No rush',  color: '#6b7280' },
];

const today = new Date().toISOString().slice(0, 10);

interface Props { onSuccess?: () => void; }

export default function CleaningRequestForm({ onSuccess }: Props) {
  const userName = useAuthStore(s => s.userName);

  
  const [searchQuery,   setSearchQuery]   = useState(userName ?? '');
  const [searchResults, setSearchResults] = useState<PublicReservationDto[]>([]);
  const [searching,     setSearching]     = useState(false);
  const [selected,      setSelected]      = useState<PublicReservationDto | null>(null);

  
  const [requestType,    setRequestType]    = useState<CleaningRequestType>('GuestRequest');
  const [scheduledDate,  setScheduledDate]  = useState(today);
  const [timeSlot,       setTimeSlot]       = useState('');
  const [priority,       setPriority]       = useState(3);

  const [loading,   setLoading]   = useState(false);
  const [error,     setError]     = useState<string | null>(null);
  const [done,      setDone]      = useState(false);
  const [conflict,  setConflict]  = useState(false);

  
  useEffect(() => {
    if (userName) runSearch(userName);
  }, []);

  async function runSearch(q: string) {
    if (!q.trim() || q.trim().length < 2) return;
    setSearching(true);
    try {
      const results = await searchReservationsPublic(q.trim());
      
      setSearchResults(results.filter(r => r.status === 'CheckedIn'));
    } catch {
      setSearchResults([]);
    } finally {
      setSearching(false);
    }
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!selected) { setError('Please select a reservation first.'); return; }
    setLoading(true);
    setError(null);
    try {
      await requestCleaning({
        roomId: selected.roomId,
        requestType,
        scheduledDate: `${scheduledDate}T00:00:00`,
        scheduledTime: timeSlot ? `${scheduledDate}T${timeSlot}:00` : undefined,
        priority,
      });
      setDone(true);
      onSuccess?.();
    } catch (err: unknown) {
      const ax = err as { response?: { status?: number; data?: { error?: { message?: string } | string } } };
      if (ax?.response?.status === 409) {
        setConflict(true);
      } else {
        const raw = ax?.response?.data?.error;
        const msg = typeof raw === 'string' ? raw : (raw as { message?: string } | undefined)?.message;
        setError(msg ?? (err instanceof Error ? err.message : 'Failed to submit request'));
      }
    } finally {
      setLoading(false);
    }
  }

  const fmt = (d: string) => new Date(d).toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric', year: 'numeric' });

  function resetForm() {
    setDone(false); setConflict(false); setSelected(null);
    setRequestType('GuestRequest'); setScheduledDate(today); setTimeSlot(''); setPriority(3);
  }

  if ((done || conflict) && selected) {
    const slot = TIME_SLOTS.find(s => s.time === timeSlot);
    const pri  = PRIORITIES.find(p => p.value === priority);
    const type = REQUEST_TYPES.find(t => t.value === requestType);

    return (
      <div style={{ textAlign: 'center', padding: '32px 16px' }}>
        <div style={{ fontSize: 48, marginBottom: 12 }}>{conflict ? '⚠️' : '✅'}</div>
        <h2 style={{ margin: '0 0 8px', fontSize: 20, fontWeight: 700 }}>
          {conflict ? 'Already in progress' : 'Request submitted!'}
        </h2>
        <p style={{ color: 'var(--c-muted)', marginBottom: 24 }}>
          {conflict
            ? `Room ${selected.roomNumber} already has an active cleaning task. Staff will attend to it shortly.`
            : `Cleaning staff have been notified for Room ${selected.roomNumber}.`}
        </p>

        <div className="info-card" style={{ textAlign: 'left', marginBottom: 24 }}>
          <div className="info-row">
            <span className="info-label">Room</span>
            <span className="info-value">{selected.roomNumber} - {selected.roomType}</span>
          </div>
          {!conflict && <>
            <div className="info-row">
              <span className="info-label">Type</span>
              <span className="info-value">{type?.label}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Date</span>
              <span className="info-value">{new Date(scheduledDate).toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' })}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Time</span>
              <span className="info-value">{slot?.label ?? 'Any time'}</span>
            </div>
            <div className="info-row">
              <span className="info-label">Priority</span>
              <span className="info-value" style={{ color: pri?.color, fontWeight: 600 }}>{pri?.label}</span>
            </div>
          </>}
          <div className="info-row">
            <span className="info-label">Status</span>
            <span className="info-value" style={{ fontWeight: 600, color: conflict ? '#d97706' : '#16a34a' }}>
              {conflict ? 'Task already exists' : 'Submitted'}
            </span>
          </div>
        </div>

        <button className="btn btn-secondary" onClick={resetForm}>
          {conflict ? 'Back' : 'Submit another request'}
        </button>
      </div>
    );
  }

  return (
    <div>
      <h2 className="step-title">Request Room Cleaning</h2>
      <p className="body-text">Select your reservation, then choose when and what kind of cleaning you need.</p>

      {error && <div className="alert alert-error" style={{ marginBottom: 16 }}><span className="alert-icon">⚠</span><span className="alert-message">{error}</span></div>}

      {}
      <p className="section-subtitle">1. Your reservation</p>

      {selected ? (
        <div style={{ border: '2px solid var(--c-primary)', borderRadius: 10, padding: '14px 16px', marginBottom: 20, display: 'flex', alignItems: 'center', justifyContent: 'space-between', background: 'var(--c-surface)' }}>
          <div>
            <div style={{ fontWeight: 700, fontSize: 15 }}>Room {selected.roomNumber} - {selected.roomType}</div>
            <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 2 }}>
              {fmt(selected.checkInDate)} → {fmt(selected.checkOutDate)} · {selected.numberOfGuests} guest{selected.numberOfGuests !== 1 ? 's' : ''}
            </div>
            <div style={{ fontSize: 12, color: 'var(--c-primary)', marginTop: 2, fontWeight: 600 }}>{selected.qrCode}</div>
          </div>
          <button type="button" className="btn btn-ghost" style={{ fontSize: 13 }} onClick={() => setSelected(null)}>
            Change
          </button>
        </div>
      ) : (
        <div style={{ marginBottom: 20 }}>
          <div style={{ display: 'flex', gap: 8, marginBottom: 12 }}>
            <input
              type="text"
              placeholder="Search by email, name, or confirmation code…"
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && (e.preventDefault(), runSearch(searchQuery))}
              style={{ flex: 1, padding: '9px 13px', borderRadius: 8, border: '1px solid var(--c-border)', fontSize: 14, background: 'var(--c-surface)' }}
            />
            <button type="button" className="btn btn-secondary" disabled={searching} onClick={() => runSearch(searchQuery)}>
              {searching ? '…' : 'Search'}
            </button>
          </div>

          {searchResults.length === 0 && !searching && searchQuery.length > 1 && (
            <p style={{ fontSize: 13, color: 'var(--c-muted)' }}>No active reservations found. Cleaning can only be requested for checked-in reservations.</p>
          )}

          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            {searchResults.map(r => (
              <button
                key={r.id}
                type="button"
                onClick={() => setSelected(r)}
                style={{ textAlign: 'left', background: 'var(--c-surface)', border: '1px solid var(--c-border)', borderRadius: 10, padding: '12px 16px', cursor: 'pointer', transition: 'border-color .15s' }}
                onMouseEnter={e => (e.currentTarget.style.borderColor = 'var(--c-primary)')}
                onMouseLeave={e => (e.currentTarget.style.borderColor = 'var(--c-border)')}
              >
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <div>
                    <span style={{ fontWeight: 700, fontSize: 15 }}>Room {r.roomNumber}</span>
                    <span style={{ marginLeft: 8, fontSize: 13, color: 'var(--c-muted)' }}>{r.roomType}</span>
                  </div>
                  <span style={{ fontSize: 12, fontWeight: 600, color: 'var(--c-primary)' }}>{r.qrCode}</span>
                </div>
                <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 4 }}>
                  {fmt(r.checkInDate)} → {fmt(r.checkOutDate)}
                </div>
              </button>
            ))}
          </div>
        </div>
      )}

      {}
      {selected && (
        <form onSubmit={handleSubmit} className="form-grid">
          <div>
            <p className="section-subtitle">2. Type of cleaning</p>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              {REQUEST_TYPES.map(t => (
                <label key={t.value} className={`room-option${requestType === t.value ? ' selected' : ''}`}>
                  <input type="radio" name="reqType" value={t.value} checked={requestType === t.value}
                    onChange={() => setRequestType(t.value)} />
                  <div className="room-option-body">
                    <span className="room-number">{t.label}</span>
                    <span className="room-type">{t.desc}</span>
                  </div>
                </label>
              ))}
            </div>
          </div>

          <div>
            <p className="section-subtitle">3. When</p>
            <div className="form-row">
              <div className="form-field">
                <label htmlFor="sched-date">Date</label>
                <input id="sched-date" type="date" value={scheduledDate} min={today}
                  onChange={e => setScheduledDate(e.target.value)} required />
              </div>
              <div className="form-field">
                <label htmlFor="time-slot">Preferred time</label>
                <select id="time-slot" value={timeSlot} onChange={e => setTimeSlot(e.target.value)}>
                  <option value="">Any time</option>
                  {TIME_SLOTS.map(s => (
                    <option key={s.time} value={s.time}>{s.label}</option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          <div>
            <p className="section-subtitle">4. Priority</p>
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
              {PRIORITIES.map(p => (
                <button
                  key={p.value}
                  type="button"
                  onClick={() => setPriority(p.value)}
                  style={{
                    padding: '8px 18px',
                    borderRadius: 20,
                    border: `2px solid ${priority === p.value ? p.color : 'var(--c-border)'}`,
                    background: priority === p.value ? p.color : 'var(--c-surface)',
                    color: priority === p.value ? '#fff' : 'var(--c-text)',
                    fontWeight: 600,
                    fontSize: 13,
                    cursor: 'pointer',
                    transition: 'all .15s',
                  }}
                >
                  {p.label}
                </button>
              ))}
            </div>
          </div>

          <div className="btn-row">
            <button type="submit" className="btn btn-primary btn-large" disabled={loading}>
              {loading ? 'Submitting…' : 'Submit cleaning request'}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
