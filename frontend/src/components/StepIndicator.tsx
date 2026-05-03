import type { WizardStep } from '../types/checkIn';

const STEPS: { key: WizardStep; label: string; icon: string }[] = [
  { key: 'search',       label: 'Reservation', icon: '🔍' },
  { key: 'identity',     label: 'Identity',    icon: '🪪' },
  { key: 'room',         label: 'Room',        icon: '🛏️' },
  { key: 'payment',      label: 'Payment',     icon: '💳' },
  { key: 'confirmation', label: 'Done',        icon: '✅' },
];

const ORDER: WizardStep[] = ['search', 'identity', 'room', 'payment', 'confirmation'];

export default function StepIndicator({ current }: { current: WizardStep }) {
  const currentIndex = ORDER.indexOf(current);
  return (
    <div className="step-indicator">
      {STEPS.map((step, idx) => {
        const state = idx < currentIndex ? 'done' : idx === currentIndex ? 'active' : 'pending';
        return (
          <div key={step.key} className={`step-item step-${state}`}>
            <div className="step-circle">{state === 'done' ? '✓' : step.icon}</div>
            <span className="step-label">{step.label}</span>
            {idx < STEPS.length - 1 && <div className="step-line" />}
          </div>
        );
      })}
    </div>
  );
}
