import { useState } from 'react';
import type { ReservationDto, IdentityData, IdDocumentType } from '../../../types/checkIn';
import Alert from '../../Alert';

interface Props {
  reservation: ReservationDto;
  onVerified: (data: IdentityData) => void;
  onCancel: () => void;
}

const DOCUMENT_TYPES: { value: IdDocumentType; label: string }[] = [
  { value: 'Passport',      label: 'Passport' },
  { value: 'NationalId',    label: 'National ID' },
  { value: 'DriverLicense', label: "Driver's License" },
];

export default function VerifyIdentity({ reservation, onVerified, onCancel }: Props) {
  const [idType, setIdType]     = useState<IdDocumentType>('Passport');
  const [idNumber, setIdNumber] = useState('');
  const [idExpiry, setIdExpiry] = useState('');
  const [error, setError]       = useState<string | null>(null);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!idNumber.trim() || !idExpiry) return;
    if (new Date(idExpiry) < new Date()) {
      setError('ID document is expired. Check-in cannot proceed with an expired document.');
      return;
    }
    onVerified({ idType, idNumber: idNumber.trim(), idExpiry: new Date(idExpiry).toISOString() });
  }

  const { guest } = reservation;

  return (
    <div className="step-content">
      <h2 className="step-title">Verify Guest Identity</h2>
      <p className="body-text">
        Verify the identity of <strong>{guest.firstName} {guest.lastName}</strong> by checking their government-issued ID.
      </p>

      <div className="info-card">
        <div className="info-row">
          <span className="info-label">Guest</span>
          <span className="info-value">{guest.firstName} {guest.lastName}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Email</span>
          <span className="info-value">{guest.email}</span>
        </div>
        {guest.phone && (
          <div className="info-row">
            <span className="info-label">Phone</span>
            <span className="info-value">{guest.phone}</span>
          </div>
        )}
        <div className="info-row">
          <span className="info-label">Confirmation</span>
          <span className="info-value">{reservation.confirmationNumber}</span>
        </div>
      </div>

      {error && <Alert type="error" message={error} onDismiss={() => setError(null)} />}

      <form onSubmit={handleSubmit} className="form-grid">
        <div className="form-field">
          <label htmlFor="doc-type">Document Type *</label>
          <select id="doc-type" value={idType} onChange={e => setIdType(e.target.value as IdDocumentType)}>
            {DOCUMENT_TYPES.map(dt => (
              <option key={dt.value} value={dt.value}>{dt.label}</option>
            ))}
          </select>
        </div>

        <div className="form-row">
          <div className="form-field">
            <label htmlFor="doc-number">Document Number *</label>
            <input
              id="doc-number"
              required
              value={idNumber}
              onChange={e => setIdNumber(e.target.value)}
              placeholder="e.g. AB 123456"
              autoFocus
            />
          </div>
          <div className="form-field">
            <label htmlFor="doc-expiry">Expiry Date *</label>
            <input
              id="doc-expiry"
              type="date"
              required
              value={idExpiry}
              onChange={e => setIdExpiry(e.target.value)}
            />
          </div>
        </div>

        <div className="btn-row">
          <button type="submit" className="btn btn-primary">Verify Identity</button>
          <button type="button" className="btn btn-ghost" onClick={onCancel}>Back</button>
        </div>
      </form>
    </div>
  );
}
