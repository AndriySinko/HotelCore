
import apiClient from './client';
import type { ApiResult, CleaningTaskDto, CleaningWorkerDto, RequestCleaningCommand } from '../types';

export async function requestCleaning(command: RequestCleaningCommand): Promise<string> {
  const { data } = await apiClient.post<ApiResult<string>>('/api/cleaning/request', command);
  return data.data;
}

export async function getTasksForStaff(staffId: string): Promise<CleaningTaskDto[]> {
  const { data } = await apiClient.get<ApiResult<CleaningTaskDto[]>>(`/api/cleaning/tasks/${staffId}`);
  return data.data ?? [];
}

export async function getPendingTasks(): Promise<CleaningTaskDto[]> {
  const { data } = await apiClient.get<ApiResult<CleaningTaskDto[]>>('/api/cleaning/tasks/pending');
  return data.data ?? [];
}

export async function getCleaningWorkers(): Promise<CleaningWorkerDto[]> {
  const { data } = await apiClient.get<ApiResult<CleaningWorkerDto[]>>('/api/cleaning/workers');
  return data.data ?? [];
}

export async function assignCleaningTask(taskId: string, staffId: string): Promise<void> {
  await apiClient.post(`/api/cleaning/tasks/${taskId}/assign`, { staffId });
}

export async function completeTask(taskId: string): Promise<void> {
  await apiClient.post(`/api/cleaning/tasks/${taskId}/complete`);
}

export async function verifyTask(taskId: string): Promise<void> {
  await apiClient.post(`/api/cleaning/tasks/${taskId}/verify`);
}

export async function cancelTask(taskId: string, reason: string): Promise<void> {
  await apiClient.post(`/api/cleaning/tasks/${taskId}/cancel`, JSON.stringify(reason));
}
