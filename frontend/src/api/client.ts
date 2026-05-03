









import axios from 'axios';

const BASE_URL = import.meta.env.VITE_API_URL ?? '';

const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  
  
  
  withCredentials: false,
});


apiClient.interceptors.request.use((config) => {
  const raw = localStorage.getItem('auth');
  if (raw) {
    try {
      const { token } = JSON.parse(raw) as { token: string };
      if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
    } catch {
      
    }
  }
  return config;
});


apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const isLoginPage = window.location.pathname === '/login';
      const hadToken    = !!localStorage.getItem('auth');
      
      if (!isLoginPage && hadToken) {
        localStorage.removeItem('auth');
        const returnTo = encodeURIComponent(window.location.pathname);
        window.location.href = `/login?returnTo=${returnTo}`;
      }
    }
    return Promise.reject(error);
  }
);

export default apiClient;
