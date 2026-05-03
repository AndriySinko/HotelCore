

export interface AuthResult {
  succeeded: boolean;
  token: string | null;
  userId: string | null;
  userName: string | null;
  role: string | null;
  error: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}



export interface ApiResult<T> {
  data: T;
  success: boolean;
  error: string | null;
}



export type ReservationStatus =
  | 'Reserved'
  | 'CheckingIn'
  | 'CheckedIn'
  | 'CheckingOut'
  | 'CheckedOut'
  | 'Cancelled';

export type RoomStatus = 'Available' | 'Reserved' | 'Occupied' | 'UnderCleaning' | 'OutOfOrder';
export type RoomType   = 'Single' | 'Double' | 'Suite' | 'Deluxe' | 'Family';
export type PaymentMethod = 'Cash' | 'CreditCard' | 'RoomBill';
export type PaymentStatus = 'Pending' | 'Completed' | 'Declined' | 'Refunded';

export interface ReservationSummaryDto {
  id: string;
  reservationId: string;
  guestName: string;
  guestEmail: string;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  status: ReservationStatus;
  isWalkIn: boolean;
}

export interface CheckInCommand {
  reservationId: string;
  idType: string;
  idNumber: string;
  idExpiry: string;
  paymentMethod: PaymentMethod;
  alternativeRoomId?: string;
}

export interface CheckInResultDto {
  success: boolean;
  keyNumber: string;
  roomNumber: string;
}

export interface CheckOutCommand {
  reservationId: string;
}

export interface CheckOutResultDto {
  success: boolean;
  guestName: string;
  totalCharged: number;
  roomNumber: string;
}

export interface CreateWalkInCommand {
  guestFirstName: string;
  guestLastName: string;
  guestEmail: string;
  guestPhone: string;
  roomId: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
}

export interface RoomDto {
  id: string;
  roomNumber: string;
  roomType: RoomType;
  floor: number;
  pricePerNight: number;
  status?: RoomStatus;
}

export interface CreateReservationCommand {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  roomId: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
}

export interface CreateReservationResultDto {
  reservationId: string;
  qrCode: string;
  roomNumber: string;
  totalPrice: number;
}



export type CleaningRequestType = 'Routine' | 'DeepClean' | 'Turnover' | 'Emergency' | 'GuestRequest';
export type CleaningTaskStatus  = 'Requested' | 'Assigned' | 'InProgress' | 'Completed' | 'Verified' | 'Cancelled' | 'Rejected';

export interface CleaningTaskDto {
  taskId: string;
  roomNumber: string;
  requestType: string;
  status: CleaningTaskStatus;
  scheduledDate: string;
  priority: number;
  assignedStaffId?: string | null;
  assignedStaffName?: string | null;
}

export interface CleaningWorkerDto {
  id: string;
  name: string;
  email: string;
}

export interface RequestCleaningCommand {
  roomId: string;
  reservationId?: string;
  requestType: CleaningRequestType;
  scheduledDate: string;
  scheduledTime?: string;
  priority: number;
}



export type ScheduleStatus    = 'Draft' | 'Published' | 'Acknowledged' | 'Closed';
export type ShiftType         = 'Morning' | 'Afternoon' | 'Night';
export type ShiftStatus       = 'Uncovered' | 'Assigned' | 'Confirmed' | 'Completed' | 'Swapped';
export type ShiftChangeStatus = 'Pending' | 'UnderReview' | 'Approved' | 'Rejected';

export interface EmployeeDto {
  id: string;
  userName: string;
  department: string;
  position: string;
  contractHoursPerWeek: number;
}

export interface ShiftDto {
  id: string;
  date: string;
  startTime: string;
  endTime: string;
  shiftType: ShiftType;
  requiredRole: string;
  status: ShiftStatus;
  staffMemberId: string | null;
  assignedEmployeeName: string | null;
}

export interface ScheduleDto {
  id: string;
  periodStart: string;
  periodEnd: string;
  status: ScheduleStatus;
  shifts: ShiftDto[];
}

export interface CreateScheduleCommand {
  periodStart: string;
  periodEnd: string;
  createdByUserId: string;
}

export interface AssignShiftCommand {
  scheduleId: string;
  staffMemberId: string;
  date: string;
  startTime: string;
  endTime: string;
  shiftType: ShiftType;
  requiredRole: string;
}
