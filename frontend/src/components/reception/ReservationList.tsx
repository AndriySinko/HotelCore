import { useState, useEffect } from 'react';
import { getActiveReservations, type ActiveReservationDto } from '../../api/receptionApi';

export default function ReservationList() {
  const [reservations, setReservations] = useState<ActiveReservationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getActiveReservations()
      .then(setReservations)
      .catch(err => setError(err instanceof Error ? err.message : 'Failed to load reservations'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="empty-state">
        <div className="spinner" />
        <div>Loading reservations…</div>
      </div>
    );
  }

  if (error) {
    return <div className="alert alert-error"><span className="alert-icon">⚠</span><span className="alert-message">{error}</span></div>;
  }

  if (reservations.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📋</div>
        <div className="empty-title">No active reservations</div>
        <div className="empty-desc">No confirmed or checked-in reservations at the moment.</div>
      </div>
    );
  }

  return (
    <div>
      <p className="results-count">{reservations.length} reservation{reservations.length !== 1 ? 's' : ''}</p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginTop: 12 }}>
        {reservations.map(r => (
          <div key={r.id} className="info-card" style={{ padding: '12px 16px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 8 }}>
              <div>
                <div style={{ fontWeight: 600, fontSize: 15 }}>{r.guestName || r.guestEmail}</div>
                <div style={{ color: 'var(--c-muted)', fontSize: 13, marginTop: 2 }}>
                  Room {r.roomNumber} · {r.roomType} ·{' '}
                  {new Date(r.checkInDate).toLocaleDateString()} – {new Date(r.checkOutDate).toLocaleDateString()}
                </div>
                {r.qrCode && (
                  <div style={{ fontSize: 12, color: 'var(--c-muted)', marginTop: 2 }}>QR: {r.qrCode}</div>
                )}
              </div>
              <span className={`badge badge-${r.status.toLowerCase()}`}>{r.status}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
