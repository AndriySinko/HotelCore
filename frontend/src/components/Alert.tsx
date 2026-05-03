interface Props {
  type: 'error' | 'warning' | 'info' | 'success';
  message: string;
  onDismiss?: () => void;
}

const ICONS = { error: '✕', warning: '⚠', info: 'ℹ', success: '✓' };

export default function Alert({ type, message, onDismiss }: Props) {
  return (
    <div className={`alert alert-${type}`} role="alert">
      <span className="alert-icon">{ICONS[type]}</span>
      <span className="alert-message">{message}</span>
      {onDismiss && (
        <button className="alert-dismiss" onClick={onDismiss} aria-label="Dismiss">×</button>
      )}
    </div>
  );
}
