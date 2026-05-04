import React, { useState, useEffect } from 'react';
import useAuthStore from '../stores/authStore';
import MyShifts from '../components/schedule/MyShifts';
import {
  createSchedule,
  getEmployees,
  assignShift,
  clearShifts,
  saveDraft,
  publishSchedule,
  getSchedule,
  getAllSchedules,
  getMyShiftsSchedule,
} from '../api/scheduleApi';
import type { EmployeeDto, ScheduleDto, ShiftType } from '../types';

const MANAGER_ROLES = ['HotelManager', 'Administrator'];

const SHIFTS = [
  { name: 'Morning',   time: '6:00–14:00',  shiftType: 'Morning' as ShiftType, start: '06:00', end: '14:00' },
  { name: 'Afternoon', time: '14:00–22:00', shiftType: 'Afternoon' as ShiftType, start: '14:00', end: '22:00' },
  { name: 'Night',     time: '22:00–6:00',  shiftType: 'Night'   as ShiftType, start: '22:00', end: '06:00' },
];

const DAYS = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

function getWeekDates(start: string): string[] {
  const base = new Date(start);
  return DAYS.map((_, i) => {
    const d = new Date(base);
    d.setDate(d.getDate() + i);
    return d.toISOString().split('T')[0];
  });
}

type CellAssignment = {
  shiftId: string;
  employeeId: string;
  employeeName: string;
  role: string;
  date: string;
  startTime: string;
  endTime: string;
  shiftType: ShiftType;
};

export default function SchedulePage() {
  const { role, userId } = useAuthStore();
  const isManager = role && MANAGER_ROLES.includes(role);

  const [managerTab, setManagerTab] = useState<'create' | 'list'>('list');
  const [allSchedules, setAllSchedules] = useState<ScheduleDto[]>([]);
  const [loadingSchedules, setLoadingSchedules] = useState(false);

  const [step, setStep]           = useState<'period' | 'assign'>('period');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate]     = useState('');
  const [scheduleId, setScheduleId]   = useState('');
  const [schedule,   setSchedule]     = useState<ScheduleDto | null>(null);
  const [employees,  setEmployees]    = useState<EmployeeDto[]>([]);
  const [creating,   setCreating]     = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);

  const [grid, setGrid]             = useState<Record<string, CellAssignment[]>>({});
  const [selectedCell, setSelectedCell] = useState<{ day: string; shift: string } | null>(null);
  const [conflicts, setConflicts]   = useState<string[]>([]);
  const [warnings,  setWarnings]    = useState<string[]>([]);
  const [showReview, setShowReview] = useState(false);

  useEffect(() => {
    if (isManager) {
      // Managers load employees to assign people to shifts.
      getEmployees().then(setEmployees).catch(() => {});
      return;
    }
    if (userId) {
      // Staff users load their own latest published schedule.
      getMyShiftsSchedule(userId).then(setSchedule).catch(() => setSchedule(null));
    }
  }, [isManager, userId]);

  useEffect(() => {
    if (!isManager || managerTab !== 'list') return;
    setLoadingSchedules(true);
    getAllSchedules().then(setAllSchedules).catch(() => setAllSchedules([])).finally(() => setLoadingSchedules(false));
  }, [isManager, managerTab]);

  function buildGridFromSchedule(s: ScheduleDto) {
    const newGrid: Record<string, CellAssignment[]> = {};
    for (const shift of s.shifts) {
      if (!shift.staffMemberId || !shift.assignedEmployeeName) continue;
      const date     = shift.date.split('T')[0];
      const dayIndex = getWeekDates(s.periodStart.split('T')[0]).indexOf(date);
      const dayName  = DAYS[dayIndex] ?? date;
      const shiftObj = SHIFTS.find(sh => sh.shiftType === shift.shiftType);
      if (!shiftObj) continue;
      const key = `${dayName}-${shiftObj.name}`;
      if (!newGrid[key]) newGrid[key] = [];
      newGrid[key].push({
        shiftId: shift.id,
        employeeId: shift.staffMemberId,
        employeeName: shift.assignedEmployeeName,
        role: shift.requiredRole,
        date,
        startTime: shift.startTime,
        endTime: shift.endTime,
        shiftType: shift.shiftType,
      });
    }
    return newGrid;
  }

  async function handleStartSchedule() {
    if (!startDate || !endDate || !userId) return;
    setCreating(true);
    setActionError(null);
    try {
      const id     = await createSchedule({ periodStart: startDate, periodEnd: endDate, createdByUserId: userId });
      const loaded = await getSchedule(id);
      setScheduleId(id);
      setSchedule(loaded);
      setGrid(buildGridFromSchedule(loaded));
      setStep('assign');
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { error?: { message?: string } | string } } };
      const raw = ax?.response?.data?.error;
      const msg = typeof raw === 'string' ? raw : (raw as { message?: string } | undefined)?.message
        ?? (err instanceof Error ? err.message : 'Failed to create schedule');
      setActionError(msg);
    } finally {
      setCreating(false);
    }
  }

  function handleAssignEmployee(emp: EmployeeDto) {
    if (!selectedCell) return;
    const { day, shift: shiftName } = selectedCell;
    const shiftObj = SHIFTS.find(s => s.name === shiftName)!;
    const dayIndex = DAYS.indexOf(day);
    const date     = getWeekDates(startDate)[dayIndex];
    const key      = `${day}-${shiftName}`;

    if ((grid[key] ?? []).some(a => a.employeeId === emp.id)) return;

    // Night shifts end on the next date when end time is earlier than start time.
    const shiftEndDate = shiftObj.end < shiftObj.start
      ? new Date(new Date(date).getTime() + 24 * 60 * 60 * 1000).toISOString().split('T')[0]
      : date;

    const assignment: CellAssignment = {
      shiftId:      '',
      employeeId:   emp.id,
      employeeName: emp.userName,
      role:         emp.position,
      date,
      startTime:    `${date}T${shiftObj.start}:00`,
      endTime:      `${shiftEndDate}T${shiftObj.end}:00`,
      shiftType:    shiftObj.shiftType,
    };

    const newGrid = { ...grid, [key]: [...(grid[key] ?? []), assignment] };
    setGrid(newGrid);
    setSelectedCell(null);
    validateGrid(newGrid);
  }

  function handleRemoveAssignment(key: string, employeeId: string) {
    const newGrid = { ...grid, [key]: (grid[key] ?? []).filter(a => a.employeeId !== employeeId) };
    setGrid(newGrid);
    validateGrid(newGrid);
  }

  function validateGrid(g: Record<string, CellAssignment[]>) {
    const empHours: Record<string, number> = {};
    Object.values(g).forEach(arr => arr.forEach(a => { empHours[a.employeeId] = (empHours[a.employeeId] ?? 0) + 8; }));
    const errs: string[] = [];
    const warns: string[] = [];
    Object.entries(empHours).forEach(([id, h]) => {
      const emp = employees.find(e => e.id === id);
      if (h > 40) errs.push(`${emp?.userName ?? id} exceeds 40 h/week (${h}h assigned)`);
      else if (emp && h > emp.contractHoursPerWeek) warns.push(`${emp.userName} assigned ${h}h (contract: ${emp.contractHoursPerWeek}h/week)`);
    });
    setConflicts(errs);
    setWarnings(warns);
  }

  function calcHours(empId: string) {
    return Object.values(grid).reduce((s, arr) => s + arr.filter(a => a.employeeId === empId).length * 8, 0);
  }

  async function handleSaveDraft() {
    if (!scheduleId) return;
    try { await saveDraft(scheduleId); }
    catch (err: unknown) { setActionError(err instanceof Error ? err.message : 'Failed to save draft'); }
  }

  async function handlePublish() {
    if (!scheduleId) return;
    setActionError(null);
    try {
      await clearShifts(scheduleId);
      for (const assignments of Object.values(grid)) {
        for (const a of assignments) {
          await assignShift(scheduleId, {
            staffMemberId: a.employeeId,
            date:          a.date,
            startTime:     a.startTime,
            endTime:       a.endTime,
            shiftType:     a.shiftType,
            requiredRole:  a.role,
          });
        }
      }
      await publishSchedule(scheduleId);
      setShowReview(false);
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { error?: { message?: string } | string } } };
      const raw = ax?.response?.data?.error;
      const msg = typeof raw === 'string' ? raw : (raw as { message?: string } | undefined)?.message
        ?? (err instanceof Error ? err.message : 'Failed to publish');
      setActionError(msg);
    }
  }

  
  if (!isManager) {
    return (
      <div>
        <div className="page-header">
          <h1 className="page-title">My Shifts</h1>
        </div>
        <div className="card">
          {schedule && userId
            ? <MyShifts shifts={schedule.shifts} staffMemberId={userId} />
            : <div className="empty-state"><div className="empty-icon">📅</div><div className="empty-title">No schedule available</div><div className="empty-desc">Your manager has not yet published a schedule.</div></div>
          }
        </div>
      </div>
    );
  }

  
  if (step === 'period') {
    const tabStyle = (active: boolean): React.CSSProperties => ({
      padding: '8px 20px', borderRadius: 6, border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14,
      background: active ? 'var(--c-primary)' : 'var(--c-surface)',
      color: active ? '#fff' : 'var(--c-text)',
    });

    return (
      <div>
        <div className="page-header">
          <h1 className="page-title">Staff Schedule</h1>
          <p className="page-subtitle">Manage work schedules for your team</p>
        </div>

        <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
          <button style={tabStyle(managerTab === 'list')} onClick={() => setManagerTab('list')}>📋 All Schedules</button>
          <button style={tabStyle(managerTab === 'create')} onClick={() => setManagerTab('create')}>➕ Create New Schedule</button>
        </div>

        {managerTab === 'create' && (
          <div className="card">
            <h2 className="step-title">📅 Create Work Schedule</h2>
            <p className="body-text">Choose the start and end dates for the scheduling period.</p>
            <div className="form-grid" style={{ marginTop: 16 }}>
              <p className="section-subtitle">Step 1 - Select Scheduling Period</p>
              <div className="form-row">
                <div className="form-field">
                  <label>Start Date</label>
                  <input type="date" value={startDate} onChange={e => setStartDate(e.target.value)} />
                </div>
                <div className="form-field">
                  <label>End Date</label>
                  <input type="date" value={endDate} onChange={e => setEndDate(e.target.value)} />
                </div>
              </div>
              {startDate && endDate && (
                <div className="info-card" style={{ padding: '12px 16px' }}>
                  <div className="info-row"><span className="info-label">Period</span><span className="info-value">{startDate} → {endDate}</span></div>
                  <div className="info-row"><span className="info-label">Duration</span><span className="info-value">7 days (1 week)</span></div>
                </div>
              )}
              {actionError && <div className="alert alert-error"><span className="alert-icon">⚠</span><span className="alert-message">{actionError}</span></div>}
              <div className="btn-row">
                <button className="btn btn-primary btn-large" onClick={handleStartSchedule} disabled={!startDate || !endDate || creating}>
                  {creating ? 'Creating…' : 'Continue to Schedule Assignment →'}
                </button>
              </div>
            </div>
          </div>
        )}

        {managerTab === 'list' && (
          <div className="card">
            <h2 className="step-title">📋 Published Schedules</h2>
            {loadingSchedules && <p className="body-text">Loading…</p>}
            {!loadingSchedules && allSchedules.length === 0 && (
              <div className="empty-state">
                <div className="empty-icon">📅</div>
                <div className="empty-title">No schedules yet</div>
                <div className="empty-desc">Create and publish a schedule to see it here.</div>
              </div>
            )}
            {!loadingSchedules && allSchedules.length > 0 && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginTop: 8 }}>
                {allSchedules.map(s => {
                  const start = s.periodStart.split('T')[0];
                  const end   = s.periodEnd.split('T')[0];
                  const byEmployee: Record<string, { name: string; shifts: typeof s.shifts }> = {};
                  for (const sh of s.shifts) {
                    if (!sh.staffMemberId || !sh.assignedEmployeeName) continue;
                    if (!byEmployee[sh.staffMemberId]) byEmployee[sh.staffMemberId] = { name: sh.assignedEmployeeName, shifts: [] };
                    byEmployee[sh.staffMemberId].shifts.push(sh);
                  }
                  return (
                    <div key={s.id} style={{ border: '1px solid var(--c-border)', borderRadius: 8, padding: 16 }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
                        <div>
                          <span style={{ fontWeight: 700, fontSize: 15 }}>{start} → {end}</span>
                          <span style={{ marginLeft: 12, fontSize: 12, padding: '2px 8px', borderRadius: 4, background: s.status === 'Published' ? '#dcfce7' : '#fef9c3', color: s.status === 'Published' ? '#16a34a' : '#92400e' }}>
                            {s.status}
                          </span>
                        </div>
                        <span style={{ fontSize: 12, color: 'var(--c-muted)' }}>{s.shifts.length} shifts</span>
                      </div>
                      {Object.values(byEmployee).length > 0 && (
                        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
                          <thead>
                            <tr style={{ background: 'var(--c-bg)' }}>
                              <th style={{ textAlign: 'left', padding: '6px 8px', border: '1px solid var(--c-border)' }}>Employee</th>
                              {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'].map(d => (
                                <th key={d} style={{ textAlign: 'center', padding: '6px 4px', border: '1px solid var(--c-border)', fontSize: 11 }}>{d}</th>
                              ))}
                            </tr>
                          </thead>
                          <tbody>
                            {Object.values(byEmployee).map(({ name, shifts: empShifts }) => {
                              const weekDates = Array.from({ length: 7 }, (_, i) => {
                                const d = new Date(start); d.setDate(d.getDate() + i); return d.toISOString().split('T')[0];
                              });
                              return (
                                <tr key={name}>
                                  <td style={{ padding: '6px 8px', border: '1px solid var(--c-border)', fontWeight: 600 }}>{name}</td>
                                  {weekDates.map(date => {
                                    const dayShift = empShifts.find(sh => sh.date.split('T')[0] === date);
                                    return (
                                      <td key={date} style={{ padding: '4px', border: '1px solid var(--c-border)', textAlign: 'center', background: dayShift ? '#f0fdf4' : 'var(--c-surface)' }}>
                                        {dayShift ? <span style={{ fontSize: 11, fontWeight: 600 }}>{dayShift.shiftType.slice(0, 3)}</span> : <span style={{ color: 'var(--c-muted)', fontSize: 10 }}>-</span>}
                                      </td>
                                    );
                                  })}
                                </tr>
                              );
                            })}
                          </tbody>
                        </table>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}
      </div>
    );
  }

  
  const totalAssigned = Object.values(grid).reduce((s, arr) => s + arr.length, 0);

  return (
    <div>
      <div className="page-header">
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 12 }}>
          <div>
            <h1 className="page-title">Staff Schedule</h1>
            <p className="page-subtitle">📅 {startDate} → {endDate}</p>
          </div>
          <div style={{ display: 'flex', gap: 8 }}>
            <button className="btn btn-ghost btn-sm" onClick={() => setStep('period')}>← Back</button>
            <button className="btn btn-ghost btn-sm" onClick={handleSaveDraft}>💾 Save Draft</button>
            <button
              className="btn btn-primary btn-sm"
              disabled={conflicts.length > 0}
              onClick={() => { if (conflicts.length === 0) setShowReview(true); }}
            >
              ✓ Confirm Schedule
            </button>
          </div>
        </div>
      </div>

      {actionError && (
        <div className="alert alert-error" style={{ marginBottom: 16 }}>
          <span className="alert-icon">⚠</span><span className="alert-message">{actionError}</span>
        </div>
      )}

      {conflicts.map((c, i) => (
        <div key={i} className="alert alert-error" style={{ marginBottom: 8 }}>
          <span className="alert-icon">✕</span><span className="alert-message"><strong>Error:</strong> {c}</span>
        </div>
      ))}
      {warnings.map((w, i) => (
        <div key={i} className="alert alert-warning" style={{ marginBottom: 8 }}>
          <span className="alert-icon">⚠</span><span className="alert-message"><strong>Warning:</strong> {w}</span>
        </div>
      ))}

      <div style={{ display: 'flex', gap: 16, alignItems: 'flex-start' }}>
        {}
        <div className="card" style={{ width: 220, flexShrink: 0 }}>
          <p className="section-subtitle">👤 Employees</p>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 6, maxHeight: 520, overflowY: 'auto' }}>
            {employees.map(emp => {
              const hours     = calcHours(emp.id);
              const isOver    = hours > 40;
              const isNear    = hours > emp.contractHoursPerWeek;
              const hoursColor = isOver ? 'var(--c-error)' : isNear ? '#d97706' : 'var(--c-success, #16a34a)';
              return (
                <div
                  key={emp.id}
                  onClick={() => selectedCell && handleAssignEmployee(emp)}
                  style={{
                    padding: '8px 10px',
                    border: `2px solid ${selectedCell ? 'var(--c-primary)' : 'var(--c-border)'}`,
                    borderRadius: 8,
                    cursor: selectedCell ? 'pointer' : 'default',
                    background: selectedCell ? 'var(--c-primary-soft, #eff6ff)' : 'var(--c-surface)',
                    opacity: selectedCell ? 1 : 0.7,
                  }}
                >
                  <div style={{ fontWeight: 600, fontSize: 13 }}>{emp.userName}</div>
                  <div style={{ fontSize: 11, color: 'var(--c-muted)' }}>{emp.position}</div>
                  <div style={{ fontSize: 11, color: hoursColor, marginTop: 2 }}>{hours}h / {emp.contractHoursPerWeek}h</div>
                </div>
              );
            })}
            {employees.length === 0 && <p className="body-text" style={{ fontSize: 12 }}>No employees found.</p>}
          </div>
          <div style={{ marginTop: 12, padding: '8px 10px', background: 'var(--c-bg)', borderRadius: 6, fontSize: 12, color: 'var(--c-muted)' }}>
            ℹ️ Click a cell, then click an employee to assign.
          </div>
        </div>

        {}
        <div className="card" style={{ flex: 1, overflowX: 'auto' }}>
          <p className="section-subtitle">Weekly Schedule Grid</p>

          {selectedCell && (
            <div className="alert" style={{ background: 'var(--c-primary-soft, #eff6ff)', border: '1px solid var(--c-primary)', borderRadius: 6, marginBottom: 12, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ fontSize: 13 }}>📌 <strong>{selectedCell.day}</strong> - {SHIFTS.find(s => s.name === selectedCell.shift)?.time}</span>
              <button className="btn btn-ghost btn-sm" onClick={() => setSelectedCell(null)}>Cancel</button>
            </div>
          )}

          <table style={{ width: '100%', borderCollapse: 'collapse', minWidth: 700, fontSize: 13 }}>
            <thead>
              <tr>
                <th style={{ border: '1px solid var(--c-border)', padding: '8px 10px', background: 'var(--c-bg)', textAlign: 'left', minWidth: 90 }}>Shift</th>
                {DAYS.map(d => (
                  <th key={d} style={{ border: '1px solid var(--c-border)', padding: '8px 6px', background: 'var(--c-bg)', textAlign: 'center', fontSize: 12 }}>{d}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {SHIFTS.map(shift => (
                <tr key={shift.name}>
                  <td style={{ border: '1px solid var(--c-border)', padding: '8px 10px', background: 'var(--c-bg)', fontWeight: 600, verticalAlign: 'top' }}>
                    <div>{shift.name}</div>
                    <div style={{ fontSize: 11, color: 'var(--c-muted)', fontWeight: 400 }}>{shift.time}</div>
                  </td>
                  {DAYS.map(day => {
                    const key         = `${day}-${shift.name}`;
                    const assignments = grid[key] ?? [];
                    const isSelected  = selectedCell?.day === day && selectedCell?.shift === shift.name;
                    return (
                      <td
                        key={key}
                        onClick={() => setSelectedCell({ day, shift: shift.name })}
                        style={{
                          border: `2px solid ${isSelected ? 'var(--c-primary)' : 'var(--c-border)'}`,
                          padding: 6,
                          verticalAlign: 'top',
                          cursor: 'pointer',
                          background: isSelected ? 'var(--c-primary-soft, #eff6ff)' : assignments.length > 0 ? '#f0fdf4' : 'var(--c-surface)',
                          minWidth: 80,
                        }}
                      >
                        {assignments.length === 0 ? (
                          <div style={{ color: 'var(--c-muted)', fontSize: 11, textAlign: 'center', padding: '12px 0' }}>+ assign</div>
                        ) : (
                          <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
                            {assignments.map(a => (
                              <div key={a.employeeId} style={{ background: 'var(--c-surface)', border: '1px solid var(--c-border)', borderRadius: 4, padding: '4px 6px', fontSize: 11 }}>
                                <div style={{ fontWeight: 600 }}>{a.employeeName}</div>
                                <div style={{ color: 'var(--c-muted)' }}>{a.role}</div>
                                <button
                                  onClick={e => { e.stopPropagation(); handleRemoveAssignment(key, a.employeeId); }}
                                  style={{ color: 'var(--c-error)', fontSize: 10, background: 'none', border: 'none', cursor: 'pointer', padding: '2px 0' }}
                                >
                                  ✕ Remove
                                </button>
                              </div>
                            ))}
                          </div>
                        )}
                      </td>
                    );
                  })}
                </tr>
              ))}
            </tbody>
          </table>

          {}
          <div className="info-card" style={{ marginTop: 16, padding: '12px 16px' }}>
            <p className="section-subtitle">📋 Summary</p>
            <div style={{ display: 'flex', gap: 24, flexWrap: 'wrap', marginTop: 8 }}>
              <div><div style={{ fontSize: 11, color: 'var(--c-muted)' }}>Total Slots</div><div style={{ fontSize: 22, fontWeight: 700 }}>{DAYS.length * SHIFTS.length}</div></div>
              <div><div style={{ fontSize: 11, color: 'var(--c-muted)' }}>Assigned</div><div style={{ fontSize: 22, fontWeight: 700, color: 'var(--c-primary)' }}>{totalAssigned}</div></div>
              <div><div style={{ fontSize: 11, color: 'var(--c-muted)' }}>Errors</div><div style={{ fontSize: 22, fontWeight: 700, color: 'var(--c-error)' }}>{conflicts.length}</div></div>
              <div><div style={{ fontSize: 11, color: 'var(--c-muted)' }}>Warnings</div><div style={{ fontSize: 22, fontWeight: 700, color: '#d97706' }}>{warnings.length}</div></div>
            </div>
          </div>
        </div>
      </div>

      {}
      {showReview && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 24, zIndex: 50 }}>
          <div className="card" style={{ maxWidth: 520, width: '100%' }}>
            <h2 className="step-title">✓ Review and Confirm Schedule</h2>
            <p className="body-text">Final review before publishing to all staff.</p>

            <div className="info-card" style={{ margin: '16px 0', padding: '12px 16px' }}>
              <div className="info-row"><span className="info-label">Period</span><span className="info-value">{startDate} → {endDate}</span></div>
              <div className="info-row"><span className="info-label">Assignments</span><span className="info-value">{totalAssigned}</span></div>
              <div className="info-row"><span className="info-label">Conflicts</span><span className="info-value" style={{ color: 'var(--c-error)' }}>{conflicts.length}</span></div>
              <div className="info-row"><span className="info-label">Warnings</span><span className="info-value" style={{ color: '#d97706' }}>{warnings.length}</span></div>
            </div>

            {actionError && <div className="alert alert-error" style={{ marginBottom: 12 }}><span className="alert-icon">⚠</span><span className="alert-message">{actionError}</span></div>}

            <div className="btn-row">
              <button className="btn btn-ghost" onClick={() => setShowReview(false)}>Back to Editing</button>
              <button className="btn btn-primary" onClick={handlePublish}>📤 Publish &amp; Notify</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
