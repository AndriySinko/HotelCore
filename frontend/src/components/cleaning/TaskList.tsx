
import { useState, useEffect, useCallback } from 'react';
import { getTasksForStaff } from '../../api/cleaningApi';
import TaskStatusCard from './TaskStatusCard';
import type { CleaningTaskDto } from '../../types';

interface Props {
  staffId: string;
  userRole?: string;
}

export default function TaskList({ staffId, userRole }: Props) {
  const [tasks, setTasks] = useState<CleaningTaskDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const loadTasks = useCallback(async () => {
    setLoading(true);
    setErrorMessage(null);
    try {
      const fetchedTasks = await getTasksForStaff(staffId);
      setTasks(fetchedTasks);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to load tasks';
      setErrorMessage(message);
    } finally {
      setLoading(false);
    }
  }, [staffId]);

  useEffect(() => { loadTasks(); }, [loadTasks]);

  if (loading) {
    return (
      <div className="empty-state">
        <div className="spinner" />
        <div>Loading tasks…</div>
      </div>
    );
  }

  if (errorMessage) {
    return (
      <div className="alert alert-error">
        <span className="alert-icon">⚠</span>
        <span className="alert-message">{errorMessage}</span>
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">✅</div>
        <div className="empty-title">No tasks assigned</div>
        <div className="empty-desc">You have no cleaning tasks at the moment.</div>
      </div>
    );
  }

  return (
    <div>
      <p className="results-count">{tasks.length} task{tasks.length !== 1 ? 's' : ''}</p>
      <div className="task-list">
        {tasks.map((task) => (
          <TaskStatusCard
            key={task.id}
            task={task}
            userRole={userRole}
            onStatusChange={loadTasks}
          />
        ))}
      </div>
    </div>
  );
}
