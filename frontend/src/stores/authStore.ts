








import { create } from 'zustand';
import apiClient from '../api/client';
import type { AuthResult, LoginRequest } from '../types';

interface AuthState {
  token: string | null;
  userId: string | null;
  userName: string | null;
  role: string | null;
  isAuthenticated: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => void;
}

const STORAGE_KEY = 'auth';

function loadFromStorage(): Pick<AuthState, 'token' | 'userId' | 'userName' | 'role' | 'isAuthenticated'> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { token: null, userId: null, userName: null, role: null, isAuthenticated: false };
    const parsed = JSON.parse(raw) as Partial<AuthState>;
    if (!parsed.token) return { token: null, userId: null, userName: null, role: null, isAuthenticated: false };
    return {
      token: parsed.token,
      userId: parsed.userId ?? null,
      userName: parsed.userName ?? null,
      role: parsed.role ?? null,
      isAuthenticated: true,
    };
  } catch {
    return { token: null, userId: null, userName: null, role: null, isAuthenticated: false };
  }
}

const useAuthStore = create<AuthState>((set) => ({
  ...loadFromStorage(),

  login: async (credentials: LoginRequest) => {
    
    const response = await apiClient.post<AuthResult>('/api/auth/login', credentials);
    const result = response.data;

    if (!result.succeeded || !result.token) {
      throw new Error(result.error ?? 'Login failed');
    }

    const authState = {
      token: result.token,
      userId: result.userId,
      userName: result.userName,
      role: result.role,
      isAuthenticated: true,
    };

    localStorage.setItem(STORAGE_KEY, JSON.stringify(authState));
    set(authState);
  },

  logout: () => {
    localStorage.removeItem(STORAGE_KEY);
    set({ token: null, userId: null, userName: null, role: null, isAuthenticated: false });
  },
}));

export default useAuthStore;
