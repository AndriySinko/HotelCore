







import { useState, type FormEvent } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import useAuthStore from '../stores/authStore';

const ROLE_LANDING: Record<string, string> = {
  Guest:          '/guest',
  Receptionist:   '/reception',
  CleaningWorker: '/cleaning',
  Supervisor:     '/cleaning',
  HotelManager:   '/schedule',
  KitchenStaff:   '/dashboard',
  Administrator:  '/dashboard',
};

export default function LoginPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const login = useAuthStore((s) => s.login);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!email || !password) {
      setError('Please enter your email and password.');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await login({ email, password });
      const returnTo = searchParams.get('returnTo');
      if (returnTo) {
        navigate(decodeURIComponent(returnTo), { replace: true });
      } else {
        
        const currentRole = useAuthStore.getState().role;
        navigate(ROLE_LANDING[currentRole ?? ''] ?? '/dashboard', { replace: true });
      }
    } catch (err: unknown) {
      const ax = err as { response?: { status?: number; data?: { error?: string } } };
      const status = ax?.response?.status;
      const message = status === 401 || status === 400
        ? 'Invalid email or password. Please try again.'
        : (ax?.response?.data?.error ?? (err instanceof Error ? err.message : 'Login failed. Please try again.'));
      setError(message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-logo">
          <span className="login-logo-icon">🏨</span>
          <div className="login-logo-title">HotelCore HMS</div>
          <div className="login-logo-sub">Hotel Management System</div>
        </div>

        {error && (
          <div className="alert alert-error" style={{ marginBottom: 16 }}>
            <span className="alert-icon">⚠</span>
            <span className="alert-message">{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="form-grid">
          <div className="form-field">
            <label htmlFor="email">Email address</label>
            <input
              id="email"
              type="email"
              placeholder="your@email.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="email"
              required
            />
          </div>

          <div className="form-field">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              required
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary btn-large"
            disabled={loading}
            style={{ marginTop: 8 }}
          >
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 16, fontSize: 14, color: 'var(--c-muted)' }}>
          No account yet?{' '}
          <Link to="/register" style={{ color: 'var(--c-primary)', fontWeight: 600 }}>Create a guest account</Link>
        </p>
      </div>
    </div>
  );
}
