
import { useState, useEffect } from 'react';
import { getEmployees, assignShift } from '../../api/scheduleApi';
import type { EmployeeDto, ShiftType } from '../../types';

interface Props {
  scheduleId: string;
  onAssigned?: () => void;
}

const SHIFT_TYPES: ShiftType[] = ['Morning', 'Afternoon', 'Night'];

export default function ShiftAssigner({ scheduleId, onAssigned }: Props) {
  const [employees, setEmployees] = useState<EmployeeDto[]>([]);
  const [staffMemberId, setStaffMemberId] = useState('');
  const [date, setDate] = useState('');
  const [startTime, setStartTime] = useState('08:00');
  const [endTime, setEndTime] = useState('16:00');
  const [shiftType, setShiftType] = useState<ShiftType>('Morning');
  const [requiredRole, setRequiredRole] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    getEmployees().then(setEmployees).catch(() => {});
  }, []);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!staffMemberId || !date) { setError('Staff member and date are required.'); return; }
    setLoading(true); setError(null); setSuccess(false);
    try {
      await assignShift(scheduleId, { staffMemberId, date, startTime, endTime, shiftType, requiredRole });
      setSuccess(true);
      onAssigned?.();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to assign shift');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <h3 className="step-title" style={{ fontSize: 16, marginBottom: 16 }}>Assign Shift</h3>

      {error   && <div className="alert alert-error"><span className="alert-icon">⚠</span><span>{error}</span></div>}
      {success && <div className="alert alert-success"><span className="alert-icon">✓</span><span>Shift assigned successfully.</span></div>}

      <form onSubmit={handleSubmit} className="form-grid">
        <div className="form-row">
          <div className="form-field">
            <label>Staff member</label>
            <select value={staffMemberId} onChange={(e) => setStaffMemberId(e.target.value)} required>
              <option value="">Select employee…</option>
              {employees.map((emp) => (
                <option key={emp.id} value={emp.id}>{emp.userName} - {emp.position}</option>
              ))}
            </select>
          </div>
          <div className="form-field">
            <label>Date</label>
            <input type="date" value={date} onChange={(e) => setDate(e.target.value)} required />
          </div>
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>Start time</label>
            <input type="time" value={startTime} onChange={(e) => setStartTime(e.target.value)} />
          </div>
          <div className="form-field">
            <label>End time</label>
            <input type="time" value={endTime} onChange={(e) => setEndTime(e.target.value)} />
          </div>
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>Shift type</label>
            <select value={shiftType} onChange={(e) => setShiftType(e.target.value as ShiftType)}>
              {SHIFT_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
            </select>
          </div>
          <div className="form-field">
            <label>Required role</label>
            <input type="text" placeholder="e.g. CleaningWorker" value={requiredRole} onChange={(e) => setRequiredRole(e.target.value)} />
          </div>
        </div>

        <div className="btn-row">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Assigning…' : 'Assign shift'}
          </button>
        </div>
      </form>
    </div>
  );
}
