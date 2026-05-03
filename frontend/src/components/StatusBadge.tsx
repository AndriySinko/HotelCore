





// maps every status value from all three modules to a badge class
// fallback is pending style so it never renders completely unstyled
interface Props {
  status: string;
  className?: string;
}

const STATUS_CLASS: Record<string, string> = {
  // reservation statuses
  Pending:     'badge badge-pending',
  Confirmed:   'badge badge-confirmed',
  CheckingIn:  'badge badge-confirmed',
  CheckedIn:   'badge badge-checkedin',
  CheckedOut:  'badge badge-checkedout',
  Cancelled:   'badge badge-cancelled',
  NoShow:      'badge badge-cancelled',

  // room statuses
  Available:     'badge badge-available',
  Reserved:      'badge badge-confirmed',
  Occupied:      'badge badge-checkedin',
  UnderCleaning: 'badge badge-inprogress',
  OutOfOrder:    'badge badge-cancelled',

  // cleaning task statuses
  Assigned:   'badge badge-assigned',
  InProgress: 'badge badge-inprogress',
  Completed:  'badge badge-completed',
  Verified:   'badge badge-verified',
  Rejected:   'badge badge-cancelled',

  // schedule and shift statuses
  Draft:      'badge badge-draft',
  Published:  'badge badge-published',
  Acknowledged: 'badge badge-checkedout',
  Closed:       'badge badge-checkedout',

  Uncovered:  'badge badge-pending',
};

export default function StatusBadge({ status, className }: Props) {
  const cls = STATUS_CLASS[status] ?? 'badge badge-pending';
  return <span className={`${cls}${className ? ` ${className}` : ''}`}>{status}</span>;
}
