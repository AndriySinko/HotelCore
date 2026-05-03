import { useState } from 'react';
import type { ReservationDto } from '../../../types/checkIn';
import { searchReservations, getReservationDetails } from '../../../api/checkInApi';
import Alert from '../../Alert';

interface Props {
  onReservationSelected: (r: ReservationDto) => void;
}

interface SearchItem {
  reservationId: string;
  guestName: string;
  guestEmail: string;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  status: string;
}

const STATUS_COLOR: Record<string, string> = {
  Reserved:   '#1d4ed8',
  CheckedIn:  '#065f46',
  CheckedOut: '#374151',
  Cancelled:  '#991b1b',
};

export default function SearchReservation({ onReservationSelected }: Props) {
  const [query,     setQuery]     = useState('');
  const [results,   setResults]   = useState<SearchItem[]>([]);
  const [searching, setSearching] = useState(false);
  const [loading,   setLoading]   = useState<string | null>(null);
  const [searched,  setSearched]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);

  async function runSearch(q: string) {
    if (!q.trim()) return;
    setError(null);
    setSearching(true);
    setSearched(false);
    try {
      const data = await searchReservations(q.trim());
      setResults(data);
      setSearched(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Search failed');
    } finally {
      setSearching(false);
    }
  }

  async function handleSelect(item: SearchItem) {
    if (!item.reservationId) {
      setError('Reservation ID is invalid in search result. Please search again.');
      return;
    }

    setError(null);
    setLoading(item.reservationId);
    try {
      const full = await getReservationDetails(item.reservationId);
      onReservationSelected(full);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load reservation details');
    } finally {
      setLoading(null);
    }
  }

  const fmt = (d: string) => new Date(d).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });

  return (
    <div className="step-content">
      <h2 className="step-title">Find Reservation</h2>
      <p className="body-text">Search by guest name, email, or confirmation code.</p>

      {error && <Alert type="error" message={error} onDismiss={() => setError(null)} />}

      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <input
          type="text"
          placeholder="Name, email, or confirmation code…"
          value={query}
          onChange={e => setQuery(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && (e.preventDefault(), runSearch(query))}
          autoFocus
          style={{ flex: 1, padding: '9px 13px', borderRadius: 8, border: '1px solid var(--c-border)', fontSize: 14, background: 'var(--c-surface)' }}
        />
        <button
          type="button"
          className="btn btn-secondary"
          disabled={searching || !query.trim()}
          onClick={() => runSearch(query)}
        >
          {searching ? '…' : 'Search'}
        </button>
      </div>

      {searched && results.length === 0 && (
        <p style={{ fontSize: 13, color: 'var(--c-muted)', textAlign: 'center', padding: '24px 0' }}>
          No reservations found for "{query}".
        </p>
      )}

      {results.length > 0 && (
        <>
          <p className="results-count">{results.length} result{results.length !== 1 ? 's' : ''} found</p>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            {results.map(r => (
              <button
                key={r.reservationId}
                type="button"
                onClick={() => handleSelect(r)}
                disabled={loading === r.reservationId}
                style={{
                  textAlign: 'left', background: 'var(--c-surface)',
                  border: '1px solid var(--c-border)', borderRadius: 10,
                  padding: '12px 16px', cursor: 'pointer', transition: 'border-color .15s',
                  display: 'flex', justifyContent: 'space-between', alignItems: 'center',
                }}
                onMouseEnter={e => (e.currentTarget.style.borderColor = 'var(--c-primary)')}
                onMouseLeave={e => (e.currentTarget.style.borderColor = 'var(--c-border)')}
              >
                <div>
                  <div style={{ fontWeight: 700, fontSize: 15 }}>{r.guestName || r.guestEmail || '—'}</div>
                  {r.guestEmail && (
                    <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 1 }}>{r.guestEmail}</div>
                  )}
                  <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 2 }}>
                    Room {r.roomNumber || '—'} · {fmt(r.checkInDate)} → {fmt(r.checkOutDate)}
                  </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 10, flexShrink: 0 }}>
                  <span style={{
                    fontSize: 11, fontWeight: 700, padding: '3px 8px', borderRadius: 20,
                    background: STATUS_COLOR[r.status] ?? '#6b7280', color: '#fff',
                  }}>{r.status}</span>
                  <span style={{ color: 'var(--c-muted)' }}>{loading === r.reservationId ? '…' : '›'}</span>
                </div>
              </button>
            ))}
          </div>
        </>
      )}
    </div>
  );
}
