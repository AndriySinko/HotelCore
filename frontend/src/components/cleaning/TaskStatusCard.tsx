
import { useState } from 'react';
import { completeTask, verifyTask, cancelTask } from '../../api/cleaningApi';
import type { CleaningTaskDto, CleaningTaskStatus } from '../../types';

const STATUS_BADGE_CLASS: Record<CleaningTaskStatus, string> = {
  Requested:  'badge badge-pending',
  Assigned:   'badge badge-assigned',
  InProgress: 'badge badge-inprogress',
  Completed:  'badge badge-completed',
  Verified:   'badge badge-verified',
  Cancelled:  'badge badge-cancelled',
  Rejected:   'badge badge-cancelled',
};

interface Props {
  task: CleaningTaskDto;
  userRole?: string;
  onStatusChange?: () => void;
}

export default function TaskStatusCard({ task, userRole, onStatusChange }: Props) {
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  async function performAction(action: () => Promise<void>) {
    setLoading(true);
    setErrorMessage(null);
    try {
      await action();
      onStatusChange?.();
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { error?: { message?: string } | string } } };
      const raw = axiosError?.response?.data?.error;
      const message = typeof raw === 'string' ? raw : (raw as { message?: string } | undefined)?.message
        ?? (err instanceof Error ? err.message : 'Action failed');
      setErrorMessage(message);
    } finally {
      setLoading(false);
    }
  }

  const canComplete = (userRole === 'CleaningWorker' || userRole === 'Supervisor' || userRole === 'Administrator')
    && (task.status === 'Assigned' || task.status === 'InProgress');

  const canVerify = (userRole === 'Supervisor' || userRole === 'Administrator')
    && task.status === 'Completed';

  const canCancel = task.status !== 'Completed' && task.status !== 'Verified' && task.status !== 'Cancelled';

  return (
    <div className="task-card">
      <div className={`task-priority priority-${task.priority}`}>{task.priority}</div>

      <div className="task-info">
        <div className="task-room">Room {task.roomNumber}</div>
        <div className="task-meta">
          {task.requestType} · {new Date(task.scheduledDate).toLocaleDateString()}
        </div>
        {task.assignedStaffName && (
          <div style={{ fontSize: 12, color: 'var(--c-muted)', marginTop: 2 }}>
            Assigned to: {task.assignedStaffName}
          </div>
        )}
        {errorMessage && (
          <div style={{ fontSize: 12, color: 'var(--c-error)', marginTop: 4 }}>{errorMessage}</div>
        )}
      </div>

      <span className={STATUS_BADGE_CLASS[task.status] ?? 'badge'}>{task.status}</span>

      <div className="task-actions">
        {canComplete && (
          <button
            className="btn btn-primary btn-sm"
            disabled={loading}
            onClick={() => performAction(() => completeTask(task.taskId))}
          >
            Complete
          </button>
        )}
        {canVerify && (
          <button
            className="btn btn-primary btn-sm"
            disabled={loading}
            onClick={() => performAction(() => verifyTask(task.taskId))}
          >
            Verify
          </button>
        )}
        {canCancel && (
          <button
            className="btn btn-ghost btn-sm"
            disabled={loading}
            onClick={() => performAction(() => cancelTask(task.taskId, 'Cancelled by staff'))}
          >
            Cancel
          </button>
        )}
      </div>
    </div>
  );
}
