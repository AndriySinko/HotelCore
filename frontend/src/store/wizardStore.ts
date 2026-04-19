import { create } from 'zustand';

type CleaningMode = 'immediate' | 'scheduled';
type PaymentMethod = 'card' | 'cash';

type PaymentDetails = {
  name: string;
  token?: string;
  last4?: string;
};

type Reservation = Record<string, unknown> | null;

type WizardStore = {
  selectedRooms: string[];
  cleaningMode: CleaningMode;
  scheduledDate: string | null;
  scheduledTime: string | null;
  paymentMethod: PaymentMethod;
  paymentDetails: PaymentDetails;
  reservation: Reservation;
  setSelectedRooms: (selectedRooms: string[]) => void;
  setCleaningMode: (cleaningMode: CleaningMode) => void;
  setScheduledDate: (scheduledDate: string | null) => void;
  setScheduledTime: (scheduledTime: string | null) => void;
  setPaymentMethod: (paymentMethod: PaymentMethod) => void;
  setPaymentDetails: (paymentDetails: PaymentDetails) => void;
  setReservation: (reservation: Reservation) => void;
  reset: () => void;
};

const initialState = {
  selectedRooms: [] as string[],
  cleaningMode: 'immediate' as CleaningMode,
  scheduledDate: null as string | null,
  scheduledTime: null as string | null,
  paymentMethod: 'card' as PaymentMethod,
  paymentDetails: { name: '' } as PaymentDetails,
  reservation: null as Reservation,
};

const useWizardStore = create<WizardStore>((set) => ({
  ...initialState,
  setSelectedRooms: (selectedRooms) => set({ selectedRooms }),
  setCleaningMode: (cleaningMode) => set({ cleaningMode }),
  setScheduledDate: (scheduledDate) => set({ scheduledDate }),
  setScheduledTime: (scheduledTime) => set({ scheduledTime }),
  setPaymentMethod: (paymentMethod) => set({ paymentMethod }),
  setPaymentDetails: (paymentDetails) => set({ paymentDetails }),
  setReservation: (reservation) => set({ reservation }),
  reset: () => set({ ...initialState }),
}));

export default useWizardStore;
export type { CleaningMode, PaymentMethod, PaymentDetails, Reservation, WizardStore };