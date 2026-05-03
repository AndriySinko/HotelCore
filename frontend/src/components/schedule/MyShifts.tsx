// personal shift view, filters by staffMemberId from the full schedule
import type { ShiftDto } from '../../types';

interface Props {
  shifts: ShiftDto[];
  staffMemberId: string;
}

const SHIFT_TYPE_COLOR: Record<string, string> = {
  Morning:   '#dbeafe',
  Afternoon: '#ede9fe',
  Night:     '#1e293b',
};

const SHIFT_TYPE_TEXT: Record<string, string> = {
  Morning:   '#1e40af',
  Afternoon: '#5b21b6',
  Night:     '#94a3b8',
};

export default function MyShifts({ shifts, staffMemberId }: Props) {
  const myShifts = shifts
    .filter((shift) => shift.staffMemberId === staffMemberId)
    .sort((shiftA, shiftB) => shiftA.date.localeCompare(shiftB.date));

  if (myShifts.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📅</div>
        <div className="empty-title">No shifts assigned</div>
        <div className="empty-desc">You have no upcoming shifts in this schedule period.</div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
      {myShifts.map((shift) => {
        const shiftDate = new Date(shift.date);
        const bgColor = SHIFT_TYPE_COLOR[shift.shiftType] ?? '#f1f5f9';
        const textColor = SHIFT_TYPE_TEXT[shift.shiftType] ?? '#475569';

        return (
          <div key={shift.id} className="my-shift-item">
            <div style={{ textAlign: 'center', minWidth: 48 }}>
              <div className="my-shift-date">{shiftDate.getDate()}</div>
              <div className="my-shift-month">{shiftDate.toLocaleString('default', { month: 'short' })}</div>
            </div>
            <div className="my-shift-info">
              <div className="my-shift-time">
                {shift.startTime.slice(0, 5)} – {shift.endTime.slice(0, 5)}
              </div>
              <div className="my-shift-type">{shift.requiredRole}</div>
            </div>
            <span
              className="badge"
              style={{ background: bgColor, color: textColor }}
            >
              {shift.shiftType}
            </span>
            <span className={`badge badge-${shift.status.toLowerCase()}`}>{shift.status}</span>
          </div>
        );
      })}
    </div>
  );
}
