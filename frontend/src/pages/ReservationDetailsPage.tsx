import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { QRCodeSVG } from 'qrcode.react';
import { searchReservationsPublic, type PublicReservationDto } from '../api/receptionApi';
import useAuthStore from '../stores/authStore';
import Alert from '../components/Alert';

const STATUS_LABELS: Record<string, string> = {
  Reserved:    'Reserved',
  CheckingIn:  'Checking In',
  CheckedIn:   'Checked In',
  CheckingOut: 'Checking Out',
  CheckedOut:  'Checked Out',
  Cancelled:   'Cancelled',
};

const STATUS_COLORS: Record<string, string> = {
  Reserved:    '#1d4ed8',
  CheckingIn:  '#92400e',
  CheckedIn:   '#065f46',
  CheckingOut: '#7c3aed',
  CheckedOut:  '#374151',
  Cancelled:   '#991b1b',
};

export default function ReservationDetailsPage() {
  const { code } = useParams<{ code?: string }>();
  const userName  = useAuthStore(s => s.userName); 

  const initialQuery = code ?? userName ?? '';

  const [query,    setQuery]    = useState(initialQuery);
  const [results,  setResults]  = useState<PublicReservationDto[]>([]);
  const [loading,  setLoading]  = useState(false);
  const [error,    setError]    = useState<string | null>(null);
  const [searched, setSearched] = useState(false);

  useEffect(() => {
    if (initialQuery) runSearch(initialQuery);
  }, []);

  async function runSearch(q: string) {
    if (!q.trim()) return;
    setLoading(true);
    setError(null);
    setSearched(true);
    try {
      const data = await searchReservationsPublic(q.trim());
      setResults(data);
    } catch {
      setError('Search failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  function handleSearch(e: React.FormEvent) {
    e.preventDefault();
    runSearch(query);
  }

  return (
    <div style={{ minHeight: '100vh', background: 'var(--c-bg)', padding: '32px 16px 64px' }}>
      <div style={{ maxWidth: 640, margin: '0 auto' }}>

        {}
        <div style={{ textAlign: 'center', marginBottom: 32 }}>
          <div style={{ fontSize: 24, fontWeight: 700, color: 'var(--c-text)' }}>🏨 HotelCore HMS</div>
          <h1 style={{ margin: '8px 0 4px', fontSize: 22, fontWeight: 700, color: 'var(--c-text)' }}>
            Reservation Lookup
          </h1>
          <p style={{ margin: 0, fontSize: 14, color: 'var(--c-muted)' }}>
            Search by confirmation code, guest name, or email
          </p>
        </div>

        {}
        <form onSubmit={handleSearch} style={{ display: 'flex', gap: 8, marginBottom: 24 }}>
          <input
            type="text"
            placeholder="HC-XXXXXX, guest name, or email…"
            value={query}
            onChange={e => setQuery(e.target.value)}
            style={{ flex: 1, padding: '10px 14px', borderRadius: 8, border: '1px solid var(--c-border)', fontSize: 14, background: 'var(--c-surface)' }}
          />
          <button type="submit" className="btn btn-primary" disabled={loading || !query.trim()}>
            {loading ? 'Searching…' : 'Search'}
          </button>
        </form>

        {error && <Alert type="error" message={error} onDismiss={() => setError(null)} />}

        {}
        {searched && !loading && results.length === 0 && !error && (
          <div style={{ textAlign: 'center', color: 'var(--c-muted)', fontSize: 14, padding: '32px 0' }}>
            No reservations found for "{query}".
          </div>
        )}

        {}
        {results.map(res => (
          <ReservationCard key={res.id} reservation={res} />
        ))}

        <p style={{ textAlign: 'center', marginTop: 32, fontSize: 13, color: 'var(--c-muted)' }}>
          <Link to="/guest" style={{ color: 'var(--c-primary)' }}>← Back to dashboard</Link>
        </p>
      </div>
    </div>
  );
}

function ReservationCard({ reservation: r }: { reservation: PublicReservationDto }) {
  const checkIn  = new Date(r.checkInDate);
  const checkOut = new Date(r.checkOutDate);
  const nights   = Math.round((checkOut.getTime() - checkIn.getTime()) / 86400000);
  const url      = `${window.location.origin}/reservation/${r.qrCode}`;
  const color    = STATUS_COLORS[r.status] ?? '#374151';

  const fmt = (d: Date) => d.toLocaleDateString(undefined, { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' });

  return (
    <div style={{ background: 'var(--c-surface)', border: '1px solid var(--c-border)', borderRadius: 12, overflow: 'hidden', marginBottom: 16 }}>

      {}
      <div style={{ background: color, padding: '8px 16px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <span style={{ color: '#fff', fontWeight: 600, fontSize: 13 }}>{STATUS_LABELS[r.status] ?? r.status}</span>
        <span style={{ color: 'rgba(255,255,255,.8)', fontSize: 13 }}>{r.qrCode}</span>
      </div>

      <div style={{ padding: 20, display: 'flex', gap: 20, alignItems: 'flex-start' }}>

        {}
        <div style={{ flexShrink: 0, background: '#fff', padding: 8, borderRadius: 8, border: '1px solid var(--c-border)' }}>
          <QRCodeSVG value={url} size={110} level="M" />
        </div>

        {}
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontWeight: 700, fontSize: 16, marginBottom: 4 }}>{r.guestName}</div>
          <div style={{ fontSize: 13, color: 'var(--c-muted)', marginBottom: 12 }}>{r.guestEmail}</div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '6px 16px' }}>
            <InfoRow label="Room"      value={r.roomNumber} />
            <InfoRow label="Type"      value={r.roomType} />
            <InfoRow label="Guests"    value={String(r.numberOfGuests)} />
            <InfoRow label="Duration"  value={`${nights} night${nights !== 1 ? 's' : ''}`} />
            <InfoRow label="Check-in"  value={fmt(checkIn)} />
            <InfoRow label="Check-out" value={fmt(checkOut)} />
          </div>

          <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid var(--c-border)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span style={{ fontSize: 13, color: 'var(--c-muted)' }}>Total</span>
            <span style={{ fontWeight: 700, fontSize: 17, color: 'var(--c-primary)' }}>${r.totalPrice.toFixed(2)}</span>
          </div>
        </div>
      </div>
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <div style={{ fontSize: 11, fontWeight: 600, color: 'var(--c-muted)', textTransform: 'uppercase', letterSpacing: '.04em' }}>{label}</div>
      <div style={{ fontSize: 13, fontWeight: 500, color: 'var(--c-text)' }}>{value}</div>
    </div>
  );
}
