import axios from 'axios';

const BASE_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5000';

export const apiClient = axios.create({
  baseURL: `${BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
    'ngrok-skip-browser-warning': 'true',
  },
  timeout: 15000,
});

// Attach JWT from auth store on every request.
// require() inside the interceptor breaks the circular dependency:
// apiClient → authStore → authApi → apiClient.
apiClient.interceptors.request.use((config) => {
  const { useAuthStore } = require('../store/authStore');
  const token: string | null = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  console.log(`[API] --> ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`);
  console.log('[API] Request headers:', JSON.stringify(config.headers));
  if (config.data) console.log('[API] Request body:', JSON.stringify(config.data));
  return config;
});

// Unwrap the server's ApiResult<T> envelope so callers receive T directly.
// { status: true, data: T } → resolves with T
// { status: false, error } → rejects with Error(message), same as a network failure
apiClient.interceptors.response.use(
  (response) => {
    console.log(`[API] <-- ${response.status} ${response.config.url}`);
    console.log('[API] Response body:', JSON.stringify(response.data));
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
    console.log('[API] !! Error type:', error?.constructor?.name);
    console.log('[API] !! Error message:', error?.message);
    console.log('[API] !! Error code:', error?.code);
    console.log('[API] !! Response status:', error?.response?.status);
    console.log('[API] !! Response data:', JSON.stringify(error?.response?.data));
    console.log('[API] !! Request URL:', error?.config?.baseURL + error?.config?.url);
    // Prefer the server's error message over axios's generic one.
    const message =
      error?.response?.data?.error?.message ??
      error?.response?.data?.title ??
      error?.message ??
      'Network error';
    return Promise.reject(new Error(message));
  }
);
