import { useState, useEffect, useCallback } from 'react';
import useAuthStore from '../stores/authStore';
import CleaningRequestForm from '../components/cleaning/CleaningRequestForm';
import TaskList from '../components/cleaning/TaskList';
import { getPendingTasks, getCleaningWorkers, assignCleaningTask } from '../api/cleaningApi';
import type { CleaningTaskDto, CleaningWorkerDto } from '../types';

const STAFF_ROLES    = ['CleaningWorker', 'Supervisor', 'Administrator'];
const REQUEST_ROLES  = ['Guest', 'Receptionist', 'Administrator'];
const SUPERVISOR_ROLES = ['Supervisor', 'Administrator'];

const PRIORITY_LABEL: Record<number, string> = { 1: 'Urgent', 2: 'High', 3: 'Normal', 4: 'Low', 5: 'No rush' };
const PRIORITY_COLOR: Record<number, string> = { 1: '#dc2626', 2: '#d97706', 3: '#2563eb', 4: '#16a34a', 5: '#6b7280' };

export default function CleaningPage() {
  const { role, userId } = useAuthStore();

  const isSupervisor   = role ? SUPERVISOR_ROLES.includes(role) : false;
  const isWorker       = role === 'CleaningWorker';
  const showRequestForm = !role || REQUEST_ROLES.includes(role);

  const [activeTab, setActiveTab] = useState<'assign' | 'tasks'>(isSupervisor ? 'assign' : 'tasks');

  
  const [pendingTasks,   setPendingTasks]   = useState<CleaningTaskDto[]>([]);
  const [workers,        setWorkers]        = useState<CleaningWorkerDto[]>([]);
  const [selectedWorker, setSelectedWorker] = useState<Record<string, string>>({});
  const [assigning,      setAssigning]      = useState<string | null>(null);
  const [assignError,    setAssignError]    = useState<string | null>(null);
  const [loadingPending, setLoadingPending] = useState(false);

  const loadPending = useCallback(async () => {
    setLoadingPending(true);
    try {
      const [tasks, staff] = await Promise.all([getPendingTasks(), getCleaningWorkers()]);
      setPendingTasks(tasks);
      setWorkers(staff);
    } catch {
      
    } finally {
      setLoadingPending(false);
    }
  }, []);

  useEffect(() => {
    if (isSupervisor) loadPending();
  }, [isSupervisor, loadPending]);

  async function handleAssign(taskId: string) {
    const staffId = selectedWorker[taskId];
    if (!staffId) { setAssignError('Select a worker first.'); return; }
    setAssigning(taskId);
    setAssignError(null);
    try {
      await assignCleaningTask(taskId, staffId);
      await loadPending();
      setSelectedWorker(prev => { const n = { ...prev }; delete n[taskId]; return n; });
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { error?: { message?: string } | string } } };
      const raw = ax?.response?.data?.error;
      const msg = typeof raw === 'string' ? raw : (raw as { message?: string } | undefined)?.message
        ?? (err instanceof Error ? err.message : 'Failed to assign');
      setAssignError(msg);
    } finally {
      setAssigning(null);
    }
  }

  
  if (showRequestForm && !isSupervisor && !isWorker) {
    return (
      <div>
        <div className="page-header">
          <h1 className="page-title">Cleaning</h1>
          <p className="page-subtitle">Submit a cleaning request for your room</p>
        </div>
        <div className="card">
          <CleaningRequestForm />
        </div>
      </div>
    );
  }

  
  if (isWorker && userId) {
    return (
      <div>
        <div className="page-header">
          <h1 className="page-title">My Cleaning Tasks</h1>
          <p className="page-subtitle">Your assigned cleaning tasks</p>
        </div>
        <div className="card">
          <TaskList staffId={userId} userRole={role ?? undefined} />
        </div>
      </div>
    );
  }

  
  if (isSupervisor) {
    const unassigned = pendingTasks.filter(t => t.status === 'Requested');
    const assigned   = pendingTasks.filter(t => t.status === 'Assigned');

    return (
      <div>
        <div className="page-header">
          <h1 className="page-title">Cleaning Management</h1>
          <p className="page-subtitle">Assign cleaning requests to workers</p>
        </div>

        {}
        <div style={{ display: 'flex', gap: 4, marginBottom: 16 }}>
          {(['assign', 'tasks'] as const).map(tab => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={activeTab === tab ? 'btn btn-primary btn-sm' : 'btn btn-ghost btn-sm'}
            >
              {tab === 'assign'
                ? `Pending Assignment${unassigned.length > 0 ? ` (${unassigned.length})` : ''}`
                : `In Progress${assigned.length > 0 ? ` (${assigned.length})` : ''}`}
            </button>
          ))}
        </div>

        {activeTab === 'assign' && (
          <div className="card">
            {assignError && (
              <div className="alert alert-error" style={{ marginBottom: 16 }}>
                <span className="alert-icon">⚠</span>
                <span className="alert-message">{assignError}</span>
              </div>
            )}

            {loadingPending ? (
              <div className="empty-state"><div className="spinner" /><div>Loading…</div></div>
            ) : unassigned.length === 0 ? (
              <div className="empty-state">
                <div className="empty-icon">✅</div>
                <div className="empty-title">No pending requests</div>
                <div className="empty-desc">All cleaning requests have been assigned.</div>
              </div>
            ) : (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                {unassigned.map(task => (
                  <div key={task.taskId} className="info-card" style={{ padding: '14px 16px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 12 }}>
                      <div style={{ flex: 1 }}>
                        <div style={{ fontWeight: 700, fontSize: 15 }}>Room {task.roomNumber}</div>
                        <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 2 }}>
                          {task.requestType} · {new Date(task.scheduledDate).toLocaleDateString()}
                        </div>
                        <div style={{ marginTop: 4 }}>
                          <span style={{ fontSize: 12, fontWeight: 600, color: PRIORITY_COLOR[task.priority] ?? '#6b7280' }}>
                            {PRIORITY_LABEL[task.priority] ?? `Priority ${task.priority}`}
                          </span>
                        </div>
                      </div>

                      <div style={{ display: 'flex', gap: 8, alignItems: 'center', flexWrap: 'wrap' }}>
                        <select
                          value={selectedWorker[task.taskId] ?? ''}
                          onChange={e => setSelectedWorker(prev => ({ ...prev, [task.taskId]: e.target.value }))}
                          style={{ padding: '7px 12px', borderRadius: 8, border: '1px solid var(--c-border)', fontSize: 14, background: 'var(--c-surface)', minWidth: 180 }}
                        >
                          <option value="">Select worker…</option>
                          {workers.map(w => (
                            <option key={w.id} value={w.id}>{w.name || w.email}</option>
                          ))}
                        </select>
                        <button
                          className="btn btn-primary btn-sm"
                          disabled={!selectedWorker[task.taskId] || assigning === task.taskId}
                          onClick={() => handleAssign(task.taskId)}
                        >
                          {assigning === task.taskId ? 'Assigning…' : 'Assign'}
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'tasks' && userId && (
          <div className="card">
            {assigned.length === 0 ? (
              <div className="empty-state">
                <div className="empty-icon">📋</div>
                <div className="empty-title">No tasks in progress</div>
                <div className="empty-desc">Assigned tasks will appear here.</div>
              </div>
            ) : (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                {assigned.map(task => (
                  <div key={task.taskId} className="info-card" style={{ padding: '14px 16px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 8 }}>
                      <div>
                        <div style={{ fontWeight: 700, fontSize: 15 }}>Room {task.roomNumber}</div>
                        <div style={{ fontSize: 13, color: 'var(--c-muted)', marginTop: 2 }}>
                          {task.requestType} · {new Date(task.scheduledDate).toLocaleDateString()}
                        </div>
                        {task.assignedStaffName && (
                          <div style={{ fontSize: 13, marginTop: 2 }}>Worker: <strong>{task.assignedStaffName}</strong></div>
                        )}
                      </div>
                      <span className="badge badge-assigned">Assigned</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="empty-state">
      <div className="empty-icon">🚫</div>
      <div className="empty-title">Access restricted</div>
    </div>
  );
}
