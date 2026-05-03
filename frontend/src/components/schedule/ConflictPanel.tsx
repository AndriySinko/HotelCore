
interface Props {
  errors: string[];
  onDismiss?: () => void;
}

export default function ConflictPanel({ errors, onDismiss }: Props) {
  if (errors.length === 0) return null;
  return (
    <div className="alert alert-warning" style={{ alignItems: 'flex-start', flexDirection: 'column', gap: 8 }}>
      <div style={{ display: 'flex', width: '100%', alignItems: 'center', gap: 8 }}>
        <span className="alert-icon">⚠</span>
        <strong style={{ flex: 1 }}>Schedule conflicts detected</strong>
        {onDismiss && <button className="alert-dismiss" onClick={onDismiss}>×</button>}
      </div>
      <ul style={{ paddingLeft: 24, margin: 0 }}>
        {errors.map((errorMessage, index) => (
          <li key={index} style={{ fontSize: 13, lineHeight: 1.6 }}>{errorMessage}</li>
        ))}
      </ul>
    </div>
  );
}
