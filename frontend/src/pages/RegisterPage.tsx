import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import apiClient from '../api/client';
import type { AuthResult } from '../types';
import useAuthStore from '../stores/authStore';

export default function RegisterPage() {
  const navigate = useNavigate();

  const [firstName, setFirstName] = useState('');
  const [lastName,  setLastName]  = useState('');
  const [email,     setEmail]     = useState('');
  const [password,  setPassword]  = useState('');
  const [error,     setError]     = useState<string | null>(null);
  const [loading,   setLoading]   = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const { data } = await apiClient.post<AuthResult>('/api/auth/register', {
        email,
        password,
        firstName,
        lastName,
        role: 0, 
      });
      if (!data.succeeded || !data.token) throw new Error(data.error ?? 'Registration failed');

      const authState = {
        token:           data.token,
        userId:          data.userId,
        userName:        data.userName,
        role:            data.role,
        isAuthenticated: true,
      };
      localStorage.setItem('auth', JSON.stringify(authState));
      useAuthStore.setState(authState);
      navigate('/guest', { replace: true });
    } catch (err: unknown) {
      const ax = err as { response?: { data?: string | { title?: string } } };
      const msg = typeof ax?.response?.data === 'string'
        ? ax.response.data
        : (ax?.response?.data as { title?: string })?.title
        ?? (err instanceof Error ? err.message : 'Registration failed');
      setError(msg);
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
          <div className="login-logo-sub">Create a guest account</div>
        </div>

        {error && (
          <div className="alert alert-error" style={{ marginBottom: 16 }}>
            <span className="alert-icon">⚠</span>
            <span className="alert-message">{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="form-grid">
          <div className="form-row">
            <div className="form-field">
              <label htmlFor="firstName">First name</label>
              <input id="firstName" type="text" placeholder="John"
                value={firstName} onChange={e => setFirstName(e.target.value)} required />
            </div>
            <div className="form-field">
              <label htmlFor="lastName">Last name</label>
              <input id="lastName" type="text" placeholder="Doe"
                value={lastName} onChange={e => setLastName(e.target.value)} required />
            </div>
          </div>

          <div className="form-field">
            <label htmlFor="email">Email address</label>
            <input id="email" type="email" placeholder="your@email.com"
              value={email} onChange={e => setEmail(e.target.value)}
              autoComplete="email" required />
          </div>

          <div className="form-field">
            <label htmlFor="password">Password</label>
            <input id="password" type="password" placeholder="Min. 8 characters"
              value={password} onChange={e => setPassword(e.target.value)}
              autoComplete="new-password" required />
          </div>

          <button type="submit" className="btn btn-primary btn-large" disabled={loading} style={{ marginTop: 8 }}>
            {loading ? 'Creating account…' : 'Create account'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 16, fontSize: 14, color: 'var(--c-muted)' }}>
          Already have an account?{' '}
          <Link to="/login" style={{ color: 'var(--c-primary)', fontWeight: 600 }}>Sign in</Link>
        </p>
      </div>
    </div>
  );
}
