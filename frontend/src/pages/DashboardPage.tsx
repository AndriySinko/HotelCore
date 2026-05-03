// home page after login — shows navigation cards filtered by the users role
// each role sees only the modules relevant to them: guests see booking/cleaning, staff see their tools
import { Link } from 'react-router-dom';
import useAuthStore from '../stores/authStore';

interface NavCard {
  to: string;
  icon: string;
  iconClass: string;
  title: string;
  description: string;
  // list of roles that should see this card
  roles: string[];
  external?: boolean;
}

// all possible cards — filtered down to what the current user can see
const NAV_CARDS: NavCard[] = [
  {
    to: '/reception',
    icon: '🏨',
    iconClass: 'blue',
    title: 'Reception',
    description: 'Check guests in and out, manage walk-in reservations',
    roles: ['Administrator', 'HotelManager', 'Receptionist'],
  },
  {
    to: '/cleaning',
    icon: '🧹',
    iconClass: 'green',
    title: 'Cleaning',
    description: 'Request cleaning, manage task queue and verify completed work',
    roles: ['Administrator', 'HotelManager', 'CleaningWorker', 'Supervisor', 'Receptionist'],
  },
  {
    to: '/schedule',
    icon: '📅',
    iconClass: 'purple',
    title: 'Schedule',
    description: 'View and manage staff schedules, assign shifts',
    roles: ['Administrator', 'HotelManager', 'CleaningWorker', 'KitchenStaff', 'Receptionist', 'Supervisor'],
  },
  {
    to: '/book',
    icon: '🛏️',
    iconClass: 'blue',
    title: 'Create Reservation',
    description: 'Book a room for your upcoming stay',
    roles: ['Guest'],
    external: true,
  },
  {
    to: '/cleaning',
    icon: '🧹',
    iconClass: 'green',
    title: 'Request Cleaning',
    description: 'Submit a cleaning request for your room',
    roles: ['Guest'],
  },
  {
    to: '/reservation',
    icon: '🎟️',
    iconClass: 'purple',
    title: 'My Reservations',
    description: 'Look up your reservation by confirmation code, name, or email',
    roles: ['Guest'],
  },
];

export default function DashboardPage() {
  const { userName, role } = useAuthStore();

  // if no role is set (unauthenticated edge case), show all cards — otherwise filter by role
  const visibleCards = NAV_CARDS.filter(
    (card) => !role || card.roles.includes(role)
  );

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">
          Welcome{userName ? `, ${userName}` : ''}
        </h1>
        <p className="page-subtitle">HotelCore HMS — select a module to get started</p>
      </div>

      <div className="dashboard-grid">
        {visibleCards.map((card) => (
          <Link key={card.to + card.title} to={card.to} className="dash-card">
            <div className={`dash-card-icon ${card.iconClass}`}>{card.icon}</div>
            <div>
              <div className="dash-card-title">{card.title}</div>
              <div className="dash-card-desc">{card.description}</div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
