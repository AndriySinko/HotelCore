import { create } from 'zustand';
import { User } from '../types/auth';
import { authApi } from '../api/authApi';

interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
  loginWithQrCode: (qrData: string) => Promise<void>;
  logout: () => void;
  clearError: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  token: null,
  isLoading: false,
  error: null,

  loginWithQrCode: async (qrData: string) => {
    set({ isLoading: true, error: null });
    try {
      const { user, token } = await authApi.loginWithQrCode(qrData);
      set({ user, token, isLoading: false });
    } catch (e: unknown) {
      const message = e instanceof Error ? e.message : 'Login failed';
      set({ error: message, isLoading: false });
      throw e;
    }
  },

  logout: () => set({ user: null, token: null }),

  clearError: () => set({ error: null }),
}));
