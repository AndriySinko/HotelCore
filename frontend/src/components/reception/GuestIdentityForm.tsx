








import { useState } from 'react';

const ID_TYPES = [
  { value: 'Passport',         label: 'Passport' },
  { value: 'NationalId',       label: 'National ID' },
  { value: 'DriversLicense',   label: "Driver's License" },
  { value: 'ResidencePermit',  label: 'Residence Permit' },
];

export interface IdentityFormValues {
  idType: string;
  idNumber: string;
  idExpiry: string;
}

interface Props {
  initialValues?: Partial<IdentityFormValues>;
  onSubmit: (values: IdentityFormValues) => void;
  onCancel: () => void;
  loading?: boolean;
  error?: string | null;
}

export default function GuestIdentityForm({ initialValues, onSubmit, onCancel, loading, error }: Props) {
  const [idType,   setIdType]   = useState(initialValues?.idType   ?? '');
  const [idNumber, setIdNumber] = useState(initialValues?.idNumber ?? '');
  const [idExpiry, setIdExpiry] = useState(initialValues?.idExpiry ?? '');
  const [fieldErrors, setFieldErrors] = useState<Partial<IdentityFormValues>>({});

  function validate(): boolean {
    const errors: Partial<IdentityFormValues> = {};
    if (!idType)   errors.idType   = 'Document type is required';
    if (!idNumber.trim()) errors.idNumber = 'Document number is required';
    if (!idExpiry) {
      errors.idExpiry = 'Expiry date is required';
    } else if (new Date(idExpiry) < new Date()) {
      errors.idExpiry = 'Document has expired';
    }
    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (validate()) {
      onSubmit({ idType, idNumber, idExpiry });
    }
  }

  const todayIso = new Date().toISOString().slice(0, 10);

  return (
    <div className="step-content">
      <h2 className="step-title">Verify Guest Identity</h2>
      <p className="body-text">Enter the guest's identity document details to proceed with check-in.</p>

      {error && (
        <div className="alert alert-error">
          <span className="alert-icon">⚠</span>
          <span className="alert-message">{error}</span>
        </div>
      )}

      <form onSubmit={handleSubmit} className="form-grid">
        <div className="form-field">
          <label htmlFor="id-type">Document type</label>
          <select
            id="id-type"
            value={idType}
            onChange={(e) => setIdType(e.target.value)}
          >
            <option value="">Select document type…</option>
            {ID_TYPES.map((docType) => (
              <option key={docType.value} value={docType.value}>{docType.label}</option>
            ))}
          </select>
          {fieldErrors.idType && <span className="field-error">{fieldErrors.idType}</span>}
        </div>

        <div className="form-row">
          <div className="form-field">
            <label htmlFor="id-number">Document number</label>
            <input
              id="id-number"
              type="text"
              placeholder="e.g. AB123456"
              value={idNumber}
              onChange={(e) => setIdNumber(e.target.value)}
            />
            {fieldErrors.idNumber && <span className="field-error">{fieldErrors.idNumber}</span>}
          </div>
          <div className="form-field">
            <label htmlFor="id-expiry">Expiry date</label>
            <input
              id="id-expiry"
              type="date"
              value={idExpiry}
              min={todayIso}
              onChange={(e) => setIdExpiry(e.target.value)}
            />
            {fieldErrors.idExpiry && <span className="field-error">{fieldErrors.idExpiry}</span>}
          </div>
        </div>

        <div className="btn-row">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Verifying…' : 'Verify identity'}
          </button>
          <button type="button" className="btn btn-ghost" onClick={onCancel}>
            Back
          </button>
        </div>
      </form>
    </div>
  );
}
