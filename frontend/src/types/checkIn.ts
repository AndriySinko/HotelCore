export type ReservationStatus = 'Reserved' | 'CheckedIn' | 'CheckedOut' | 'Cancelled' | 'WalkIn';
export type RoomStatus = 'Available' | 'Reserved' | 'Occupied' | 'UnderCleaning' | 'Maintenance';
export type IdDocumentType = 'Passport' | 'NationalId' | 'DriverLicense';
export type PaymentMethod = 'Cash' | 'CreditCard' | 'RoomBill';

export interface GuestDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
}

export interface RoomTypeDto {
  name: string;
  pricePerNight: number;
}

export interface RoomDto {
  id: string;
  number: string;
  floor: number;
  roomType: RoomTypeDto;
  status: RoomStatus;
}

export interface ReservationDto {
  id: string;
  confirmationNumber: string;
  guest: GuestDto;
  room: RoomDto;
  checkInDate: string;
  checkOutDate: string;
  nights: number;
  status: ReservationStatus;
  totalAmount: number;
}

export interface IdentityData {
  idType: IdDocumentType;
  idNumber: string;
  idExpiry: string;
}

export interface CheckInCompleteResult {
  reservationId: string;
  guestName: string;
  roomNumber: string;
  keyNumber: string;
}

export type WizardStep = 'search' | 'identity' | 'room' | 'payment' | 'confirmation';

export interface CheckInState {
  step: WizardStep;
  reservation: ReservationDto | null;
  identityData: IdentityData | null;
  selectedRoom: RoomDto | null;
  paymentMethod: PaymentMethod | null;
  result: CheckInCompleteResult | null;
}
