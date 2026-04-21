import { useState } from 'react';
import { searchReservations } from '../../api/receptionApi';
import type { ReservationSummaryDto } from '../../types';

const STATUS_COLOR: Record<string, string> = {
  Reserved:   '#1d4ed8',
  CheckedIn:  '#065f46',
  CheckedOut: '#374151',
  Cancelled:  '#991b1b',
};

interface Props {
  onSelect: (reservation: ReservationSummaryDto) => void;
}

export default function ReservationSearch({ onSelect }: Props) {
  const [query,     setQuery]     = useState('');
  const [results,   setResults]   = useState<ReservationSummaryDto[]>([]);
  const [searching, setSearching] = useState(false);
  const [searched,  setSearched]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);

  async function runSearch(q: string) {
    if (!q.trim()) return;
    setSearching(true);
    setError(null);
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

  const fmt = (d: string) => new Date(d).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });

  return (
    <div className="step-content">
      <h2 className="step-title">Find Reservation</h2>
      <p className="body-text">Search by guest name, email, or confirmation code.</p>

      {error && (
        <div className="alert alert-error" style={{ marginBottom: 12 }}>
          <span className="alert-icon">⚠</span>
          <span className="alert-message">{error}</span>
        </div>
      )}

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
                key={r.id}
                type="button"
                onClick={() => onSelect(r)}
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
                  <div style={{ fontWeight: 700, fontSize: 15 }}>{r.guestName || '—'}</div>
                  <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 2 }}>
                    Room {r.roomNumber} · {fmt(r.checkInDate)} → {fmt(r.checkOutDate)}
                  </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 10, flexShrink: 0 }}>
                  <span style={{
                    fontSize: 11, fontWeight: 700, padding: '3px 8px', borderRadius: 20,
                    background: STATUS_COLOR[r.status] ?? '#6b7280', color: '#fff',
                  }}>{r.status}</span>
                  <span style={{ color: 'var(--c-muted)' }}>›</span>
                </div>
              </button>
            ))}
          </div>
        </>
      )}
    </div>
  );
}
