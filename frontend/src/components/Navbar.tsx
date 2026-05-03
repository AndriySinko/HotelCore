







import { NavLink } from 'react-router-dom';
import useAuthStore from '../stores/authStore';

interface NavItem {
  to: string;
  label: string;
  roles: string[];
}

const NAV_ITEMS: NavItem[] = [
  { to: '/dashboard',  label: 'Dashboard',  roles: ['Administrator', 'HotelManager', 'Receptionist', 'CleaningWorker', 'KitchenStaff', 'Supervisor'] },
  { to: '/guest',      label: 'Home',       roles: ['Guest'] },
  { to: '/reception',  label: 'Reception',  roles: ['Administrator', 'HotelManager', 'Receptionist'] },
  { to: '/cleaning',   label: 'Cleaning',   roles: ['Administrator', 'HotelManager', 'CleaningWorker', 'Supervisor', 'Receptionist', 'Guest'] },
  { to: '/schedule',   label: 'Schedule',   roles: ['Administrator', 'HotelManager', 'CleaningWorker', 'KitchenStaff', 'Receptionist', 'Supervisor'] },
];

export default function Navbar() {
  const { userName, role, logout } = useAuthStore();

  const visibleItems = NAV_ITEMS.filter(
    (item) => !role || item.roles.includes(role)
  );

  return (
    <nav className="app-navbar">
      <NavLink to={role === 'Guest' ? '/guest' : '/dashboard'} className="navbar-brand">
        <span className="navbar-brand-icon">🏨</span>
        <span>HotelCore HMS</span>
      </NavLink>

      <div className="navbar-nav">
        {visibleItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => `navbar-link${isActive ? ' active' : ''}`}
          >
            {item.label}
          </NavLink>
        ))}
      </div>

      <div className="navbar-end">
        <span className="navbar-user" style={{ display: 'var(--navbar-user-display, inline)' }}>
          {userName ?? 'User'}
        </span>
        <button className="btn btn-ghost btn-sm" onClick={logout} title="Sign out">
          <span style={{ display: 'var(--signout-label-display, inline)' }}>Sign out</span>
          <span style={{ display: 'var(--signout-icon-display, none)' }}>✕</span>
        </button>
      </div>
    </nav>
  );
}
