import apiClient from './client';
import type { ApiResult } from '../types';
import type { ReservationDto, IdentityData, CheckInCompleteResult, PaymentMethod } from '../types/checkIn';


interface ReservationSearchItem {
  reservationId: string;
  guestName: string;
  guestEmail: string;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  status: string;
}

interface ReservationSearchItemRaw {
  reservationId?: string;
  id?: string;
  guestName?: string;
  guestEmail?: string;
  email?: string;
  roomNumber?: string;
  checkInDate?: string;
  checkOutDate?: string;
  status?: string;
}

function asText(value: unknown): string {
  return typeof value === 'string' ? value.trim() : '';
}

function extractGuid(value: string): string {
  const match = value.match(/[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/);
  return match ? match[0] : '';
}

function normalizeSearchItem(item: ReservationSearchItemRaw): ReservationSearchItem {
  // This makes different backend field names map to one clean object.
  const rawId = asText(item.reservationId) || asText(item.id);
  return {
    reservationId: extractGuid(rawId),
    guestName: asText(item.guestName),
    guestEmail: asText(item.guestEmail) || asText(item.email),
    roomNumber: asText(item.roomNumber),
    checkInDate: asText(item.checkInDate),
    checkOutDate: asText(item.checkOutDate),
    status: asText(item.status) || 'Reserved',
  };
}

function isOk(result: ApiResult<unknown> & { status?: boolean; success?: boolean }) {
  // This accepts both success and status because APIs return either one.
  return result.success ?? result.status ?? false;
}

function getErrorMessage(result: { error?: unknown }, fallback: string) {
  // This reads error text from either a plain string or an error object.
  if (typeof result.error === 'string') return result.error;
  if (
    result.error &&
    typeof result.error === 'object' &&
    'message' in result.error &&
    typeof (result.error as { message?: unknown }).message === 'string'
  ) {
    return (result.error as { message: string }).message;
  }
  return fallback;
}

export async function searchReservations(query: string): Promise<ReservationSearchItem[]> {
  const { data } = await apiClient.get<ApiResult<ReservationSearchItemRaw[]> & { status?: boolean; success?: boolean }>(
    '/api/reception/reservations/search',
    { params: { guestName: query } }
  );
  if (!isOk(data)) throw new Error(getErrorMessage(data, 'Search failed'));
  return (data.data ?? [])
    .map(normalizeSearchItem)
    .filter(x => !!x.reservationId);
}

export async function getReservationDetails(id: string): Promise<ReservationDto> {
  const { data } = await apiClient.get<ApiResult<ReservationDto> & { status?: boolean; success?: boolean }>(
    `/api/reception/reservations/${encodeURIComponent(id)}`
  );
  if (!isOk(data) || !data.data) throw new Error(getErrorMessage(data, 'Reservation not found'));
  return data.data;
}

export async function completeCheckIn(
  reservationId: string,
  identity: IdentityData,
  alternativeRoomId: string | null,
  paymentMethod: PaymentMethod
): Promise<CheckInCompleteResult> {
  const { data } = await apiClient.post<ApiResult<{ success: boolean; keyNumber: string; roomNumber: string }> & { status?: boolean; success?: boolean }>(
    '/api/reception/check-in',
    {
      reservationId,
      idType: identity.idType,
      idNumber: identity.idNumber,
      idExpiry: identity.idExpiry,
      paymentMethod,
      alternativeRoomId: alternativeRoomId ?? undefined,
    }
  );
  if (!isOk(data) || !data.data) throw new Error(getErrorMessage(data, 'Check-in failed'));
  return {
    reservationId,
    guestName: '',
    roomNumber: data.data.roomNumber,
    keyNumber: data.data.keyNumber,
  };
}
