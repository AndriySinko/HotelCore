import axios from 'axios';

const BASE_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5000';

export const apiClient = axios.create({
  baseURL: `${BASE_URL}/api`,
  headers: { 'Content-Type': 'application/json' },
  timeout: 15000,
});

// Attach JWT from auth store on every request
apiClient.interceptors.request.use((config) => {
  // Lazy import to avoid circular deps
  const { useAuthStore } = require('../store/authStore');
  const token: string | null = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Unwrap ApiResult<T> envelope — return data or throw on status: false
apiClient.interceptors.response.use(
  (response) => {
    const body = response.data;
    if (body && typeof body === 'object' && 'status' in body) {
      if (!body.status) {
        const message = body.error?.message ?? 'Request failed';
        return Promise.reject(new Error(message));
      }
      response.data = body.data;
    }
    return response;
  },
  (error) => {
    const message =
      error?.response?.data?.error?.message ??
      error?.response?.data?.title ??
      error?.message ??
      'Network error';
    return Promise.reject(new Error(message));
  }
);
