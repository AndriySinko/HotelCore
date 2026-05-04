// root component — sets up the router and defines all page routes
// public routes (login, register, book, reservation) dont require auth
// protected routes are wrapped in ProtectedRoute which redirects to /login if not authenticated
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import './index.css';
import useAuthStore from './stores/authStore';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import ReceptionPage from './pages/ReceptionPage';
import CleaningPage from './pages/CleaningPage';
import SchedulePage from './pages/SchedulePage';
import BookReservationPage from './pages/BookReservationPage';
import RegisterPage from './pages/RegisterPage';
import ReservationDetailsPage from './pages/ReservationDetailsPage';

// maps each role to the page it should land on after login
// if a role is not in this map the user falls back to /dashboard
const ROLE_LANDING: Record<string, string> = {
  Guest:          '/guest',
  Receptionist:   '/reception',
  CleaningWorker: '/cleaning',
  Supervisor:     '/cleaning',
  HotelManager:   '/schedule',
  KitchenStaff:   '/dashboard',
  Administrator:  '/dashboard',
};

// redirects the user to the right page based on their role right after login
function RoleRedirect() {
  const role = useAuthStore((s) => s.role);
  return <Navigate to={ROLE_LANDING[role ?? ''] ?? '/dashboard'} replace />;
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* public pages — no login needed */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/book" element={<BookReservationPage />} />
        <Route path="/reservation" element={<ReservationDetailsPage />} />
        <Route path="/reservation/:code" element={<ReservationDetailsPage />} />

        {/* protected pages — user must be logged in */}
        <Route element={<ProtectedRoute />}>
          <Route element={<Layout />}>
            <Route index element={<RoleRedirect />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            {/* guests land here — shows booking and cleaning request cards */}
            <Route path="/guest" element={<DashboardPage />} />
            {/* staff module pages */}
            <Route path="/reception" element={<ReceptionPage />} />
            <Route path="/cleaning" element={<CleaningPage />} />
            <Route path="/schedule" element={<SchedulePage />} />
          </Route>
        </Route>

        {/* catch-all — unknown URLs go to login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
