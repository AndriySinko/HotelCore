// global auth state — stores the JWT token and user info after login
// uses Zustand for state management and localStorage for persistence across page refreshes
// the token is picked up by the axios interceptor in client.ts and attached to every API request
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

// tries to restore auth state from localStorage on startup
// if the stored token is missing or the JSON is malformed, returns unauthenticated state
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
  // hydrate from storage so the user stays logged in after a refresh
  ...loadFromStorage(),

  login: async (credentials: LoginRequest) => {
    // sends credentials to the backend and gets back a JWT token with user info
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

    // save to localStorage so it survives a page reload
    localStorage.setItem(STORAGE_KEY, JSON.stringify(authState));
    set(authState);
  },

  logout: () => {
    // clear both the store and localStorage — user is fully logged out
    localStorage.removeItem(STORAGE_KEY);
    set({ token: null, userId: null, userName: null, role: null, isAuthenticated: false });
  },
}));

export default useAuthStore;
