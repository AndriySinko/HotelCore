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

const ROLE_LANDING: Record<string, string> = {
  Guest:          '/guest',
  Receptionist:   '/reception',
  CleaningWorker: '/cleaning',
  Supervisor:     '/cleaning',
  HotelManager:   '/schedule',
  KitchenStaff:   '/dashboard',
  Administrator:  '/dashboard',
};

function RoleRedirect() {
  const role = useAuthStore((s) => s.role);
  return <Navigate to={ROLE_LANDING[role ?? ''] ?? '/dashboard'} replace />;
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/book" element={<BookReservationPage />} />
        <Route path="/reservation" element={<ReservationDetailsPage />} />
        <Route path="/reservation/:code" element={<ReservationDetailsPage />} />

        {}
        <Route element={<ProtectedRoute />}>
          <Route element={<Layout />}>
            <Route index element={<RoleRedirect />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            {}
            <Route path="/guest" element={<DashboardPage />} />
            {}
            <Route path="/reception" element={<ReceptionPage />} />
            <Route path="/cleaning" element={<CleaningPage />} />
            <Route path="/schedule" element={<SchedulePage />} />
          </Route>
        </Route>

        {}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
