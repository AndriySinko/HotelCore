
import { create } from 'zustand';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastMessage {
  id: string;
  message: string;
  type: ToastType;
}

interface ToastState {
  toasts: ToastMessage[];
  show: (message: string, type?: ToastType) => void;
  dismiss: (id: string) => void;
}

let toastCounter = 0;

const useToastStore = create<ToastState>((set) => ({
  toasts: [],

  show: (message: string, type: ToastType = 'info') => {
    const toastId = `toast-${++toastCounter}`;
    set((state) => ({ toasts: [...state.toasts, { id: toastId, message, type }] }));
    
    setTimeout(() => {
      set((state) => ({ toasts: state.toasts.filter((toast) => toast.id !== toastId) }));
    }, 4000);
  },

  dismiss: (id: string) => {
    set((state) => ({ toasts: state.toasts.filter((toast) => toast.id !== id) }));
  },
}));

export default useToastStore;
