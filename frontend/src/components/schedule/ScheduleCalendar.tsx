
import type { ScheduleDto, ShiftDto } from '../../types';

const DAYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

function getWeekDates(startDate: Date): Date[] {
  const monday = new Date(startDate);
  monday.setDate(monday.getDate() - ((monday.getDay() + 6) % 7));
  return Array.from({ length: 7 }, (_, i) => {
    const d = new Date(monday);
    d.setDate(monday.getDate() + i);
    return d;
  });
}

function shiftsForDate(shifts: ShiftDto[], date: Date): ShiftDto[] {
  const iso = date.toISOString().slice(0, 10);
  return shifts.filter((s) => s.date.slice(0, 10) === iso);
}

interface Props {
  schedule: ScheduleDto;
  weekOffset?: number; 
}

export default function ScheduleCalendar({ schedule, weekOffset = 0 }: Props) {
  const periodStart = new Date(schedule.periodStart);
  const viewStart = new Date(periodStart);
  viewStart.setDate(periodStart.getDate() + weekOffset * 7);
  const weekDates = getWeekDates(viewStart);

  return (
    <div>
      <div className="schedule-grid">
        {}
        <div className="schedule-header-cell">Time</div>
        {weekDates.map((d, i) => (
          <div key={i} className="schedule-header-cell">
            <div>{DAYS[i]}</div>
            <div style={{ fontSize: 11, opacity: .7 }}>{d.getDate()}/{d.getMonth() + 1}</div>
          </div>
        ))}

        {}
        <div className="schedule-time-cell">Shifts</div>
        {weekDates.map((d, i) => {
          const dayShifts = shiftsForDate(schedule.shifts, d);
          return (
            <div key={i} className="schedule-day-cell">
              {dayShifts.length === 0 ? (
                <span style={{ fontSize: 11, color: 'var(--c-muted)' }}>-</span>
              ) : (
                dayShifts.map((shift) => (
                  <div
                    key={shift.id}
                    className={`shift-block ${shift.shiftType.toLowerCase()}`}
                    title={shift.assignedEmployeeName ?? 'Unassigned'}
                  >
                    <div className="shift-block-name">
                      {shift.assignedEmployeeName ?? <em>Open</em>}
                    </div>
                    <div className="shift-block-time">
                      {shift.startTime.slice(0, 5)}–{shift.endTime.slice(0, 5)}
                    </div>
                  </div>
                ))
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
