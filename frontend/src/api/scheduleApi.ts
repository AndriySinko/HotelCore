
import apiClient from './client';
import type { ApiResult, ScheduleDto, CreateScheduleCommand, AssignShiftCommand, EmployeeDto, ShiftDto } from '../types';

interface RawShiftDto extends Omit<ShiftDto, 'assignedEmployeeName'> {
  assignedEmployeeName?: string | null;
  staffName?: string | null;
}

interface RawScheduleDto extends Omit<ScheduleDto, 'shifts'> {
  shifts: RawShiftDto[];
}

function normalizeSchedule(raw: RawScheduleDto): ScheduleDto {
  return {
    ...raw,
    shifts: (raw.shifts ?? []).map(shift => ({
      ...shift,
      assignedEmployeeName: shift.assignedEmployeeName ?? shift.staffName ?? null,
    })),
  };
}

export async function createSchedule(command: CreateScheduleCommand): Promise<string> {
  const { data } = await apiClient.post<ApiResult<string>>('/api/schedule', command);
  return data.data;
}

export async function assignShift(scheduleId: string, command: Omit<AssignShiftCommand, 'scheduleId'>): Promise<string> {
  const { data } = await apiClient.post<ApiResult<string>>(`/api/schedule/${scheduleId}/shifts`, command);
  return data.data;
}

export async function saveDraft(scheduleId: string): Promise<void> {
  await apiClient.post(`/api/schedule/${scheduleId}/save-draft`);
}

export async function publishSchedule(scheduleId: string): Promise<void> {
  await apiClient.post(`/api/schedule/${scheduleId}/publish`);
}

export async function getSchedule(scheduleId: string): Promise<ScheduleDto> {
  const { data } = await apiClient.get<ApiResult<RawScheduleDto>>(`/api/schedule/${scheduleId}`);
  return normalizeSchedule(data.data);
}

export async function getEmployees(department?: string): Promise<EmployeeDto[]> {
  const params = department ? { department } : {};
  const { data } = await apiClient.get<ApiResult<EmployeeDto[]>>('/api/schedule/employees', { params });
  return data.data;
}

export async function getAllSchedules(): Promise<ScheduleDto[]> {
  const { data } = await apiClient.get<ApiResult<RawScheduleDto[]>>('/api/schedule');
  return (data.data ?? []).map(normalizeSchedule);
}

export async function clearShifts(scheduleId: string): Promise<void> {
  await apiClient.delete(`/api/schedule/${scheduleId}/shifts`);
}

export async function getMyShiftsSchedule(staffId: string): Promise<ScheduleDto | null> {
  const { data } = await apiClient.get<ApiResult<RawScheduleDto | null>>(`/api/schedule/my-shifts/${staffId}`);
  return data.data ? normalizeSchedule(data.data) : null;
}
