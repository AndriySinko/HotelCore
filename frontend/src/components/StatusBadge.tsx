






interface Props {
  status: string;
  className?: string;
}

const STATUS_CLASS: Record<string, string> = {
  
  Pending:     'badge badge-pending',
  Confirmed:   'badge badge-confirmed',
  CheckingIn:  'badge badge-confirmed',
  CheckedIn:   'badge badge-checkedin',
  CheckedOut:  'badge badge-checkedout',
  Cancelled:   'badge badge-cancelled',
  NoShow:      'badge badge-cancelled',

  
  Available:     'badge badge-available',
  Reserved:      'badge badge-confirmed',
  Occupied:      'badge badge-checkedin',
  UnderCleaning: 'badge badge-inprogress',
  OutOfOrder:    'badge badge-cancelled',

  
  Assigned:   'badge badge-assigned',
  InProgress: 'badge badge-inprogress',
  Completed:  'badge badge-completed',
  Verified:   'badge badge-verified',
  Rejected:   'badge badge-cancelled',

  
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
