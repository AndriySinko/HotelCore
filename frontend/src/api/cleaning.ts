import { mockResponse } from './client';

export type ReservationRoom = {
  room: string;
  type: string;
  checkOut: string;
  badge?: string;
  muted?: boolean;
};

export type ReservationData = {
  guestName: string;
  roomType: string;
  roomNumber: string;
  confirmation: string;
  checkOut: string;
  rooms: ReservationRoom[];
};

export type CreateCleaningPayload = {
  confirmation: string;
  selectedRooms: string[];
  cleaningMode: 'immediate' | 'scheduled';
  scheduledDate: string | null;
  scheduledTime: string | null;
};

export type CreateCleaningResponse = {
  requestId: string;
  status: 'created';
};

const mockReservation: ReservationData = {
  guestName: 'John Smith',
  roomType: 'Suite',
  roomNumber: '305',
  confirmation: 'RES492',
  checkOut: '2026-04-02',
  rooms: [
    { room: 'Room 101', type: 'Single', checkOut: '2026-03-30' },
    { room: 'Room 102', type: 'Double', checkOut: '2026-03-28', badge: 'Active Request', muted: true },
    { room: 'Room 203', type: 'Suite', checkOut: '2026-03-29' },
    { room: 'Room 305', type: 'Deluxe', checkOut: '2026-04-02' },
  ],
};

const allSlots = ['09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00'];

export async function getReservation(): Promise<ReservationData> {
  return mockResponse({
    ...mockReservation,
    rooms: mockReservation.rooms.map((room) => ({ ...room })),
  });
}

export async function getTimeSlots(date: string): Promise<string[]> {
  if (!date) {
    return mockResponse([]);
  }

  const daySeed = Number(date.split('-')[2] ?? '1');
  const filteredSlots = allSlots.filter((_, index) => (index + daySeed) % 3 !== 0);
  return mockResponse(filteredSlots);
}

export async function createCleaning(payload: CreateCleaningPayload): Promise<CreateCleaningResponse> {
  const suffix = payload.selectedRooms.length.toString().padStart(2, '0');
  return mockResponse({
    requestId: `CLN-${payload.confirmation}-${suffix}`,
    status: 'created',
  });
}
