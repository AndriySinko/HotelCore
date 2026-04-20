





import apiClient from './client';
import type {
  ApiResult,
  ReservationSummaryDto,
  CheckInCommand,
  CheckInResultDto,
  CheckOutResultDto,
  CreateWalkInCommand,
  CreateReservationCommand,
  CreateReservationResultDto,
  RoomDto,
} from '../types';

export async function searchReservations(query: string): Promise<ReservationSummaryDto[]> {
  const { data } = await apiClient.get<ApiResult<ReservationSummaryDto[]>>(
    '/api/reception/reservations/search',
    { params: { guestName: query } }
  );
  if (!data.success) throw new Error(data.error ?? 'Search failed');
  return data.data ?? [];
}

export async function createWalkIn(command: CreateWalkInCommand): Promise<string> {
  const { data } = await apiClient.post<ApiResult<string>>(
    '/api/reception/reservations/walk-in',
    command
  );
  return data.data;
}

export async function checkIn(command: CheckInCommand): Promise<CheckInResultDto> {
  const { data } = await apiClient.post<ApiResult<CheckInResultDto>>(
    '/api/reception/check-in',
    command
  );
  return data.data;
}

export async function checkOut(reservationId: string): Promise<CheckOutResultDto> {
  const { data } = await apiClient.post<ApiResult<CheckOutResultDto>>(
    '/api/reception/check-out',
    { reservationId }
  );
  return data.data;
}

export interface ActiveReservationDto {
  id: string;
  guestName: string;
  guestEmail: string;
  roomNumber: string;
  roomType: string;
  checkInDate: string;
  checkOutDate: string;
  status: string;
  qrCode: string;
}

export async function getActiveReservations(): Promise<ActiveReservationDto[]> {
  const { data } = await apiClient.get<ApiResult<ActiveReservationDto[]>>('/api/reception/reservations');
  return data.data;
}

export async function getAllRooms(): Promise<RoomDto[]> {
  const { data } = await apiClient.get<ApiResult<RoomDto[]>>('/api/reception/rooms');
  return data.data;
}

export async function getAvailableRooms(from: string, to: string): Promise<RoomDto[]> {
  const { data } = await apiClient.get<ApiResult<RoomDto[]>>(
    '/api/Reservation/rooms/available',
    { params: { checkIn: from, checkOut: to } }
  );
  return data.data;
}

export interface PublicReservationDto {
  id: string;
  qrCode: string;
  guestName: string;
  guestEmail: string;
  roomId: string;
  roomNumber: string;
  roomType: string;
  numberOfGuests: number;
  checkInDate: string;
  checkOutDate: string;
  status: string;
  totalPrice: number;
}

export async function searchReservationsPublic(query: string): Promise<PublicReservationDto[]> {
  const { data } = await apiClient.get<ApiResult<PublicReservationDto[]>>(
    '/api/public/reservations/search',
    { params: { q: query } }
  );
  return data.data ?? [];
}

export async function createReservation(
  command: CreateReservationCommand
): Promise<CreateReservationResultDto> {
  const { data } = await apiClient.post<ApiResult<CreateReservationResultDto>>(
    '/api/Reservation',
    command
  );
  if (!data.data) throw new Error(data.error ?? 'Failed to create reservation');
  return data.data;
}
