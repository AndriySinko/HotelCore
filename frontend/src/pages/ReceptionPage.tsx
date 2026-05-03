// main page for reception staff — three tabs: check-in, check-out, and reservation list
// tab state is local — switching tabs doesn't reload data, each component manages its own fetch
import { useState } from 'react';
import CheckInWizard from '../components/reception/CheckInWizard';
import CheckOutForm from '../components/reception/CheckOutForm';
import ReservationList from '../components/reception/ReservationList';

type Tab = 'checkin' | 'checkout' | 'reservations';

export default function ReceptionPage() {
  // default to check-in since thats the most common operation at the front desk
  const [activeTab, setActiveTab] = useState<Tab>('checkin');

  return (
    <div>
      <div className="page-header">
        <h1 className="page-title">Reception</h1>
        <p className="page-subtitle">Manage guest arrivals and departures</p>
      </div>

      <div className="tabs-bar">
        <button
          className={`tab-btn${activeTab === 'checkin' ? ' active' : ''}`}
          onClick={() => setActiveTab('checkin')}
        >
          Check-In
        </button>
        <button
          className={`tab-btn${activeTab === 'checkout' ? ' active' : ''}`}
          onClick={() => setActiveTab('checkout')}
        >
          Check-Out
        </button>
        <button
          className={`tab-btn${activeTab === 'reservations' ? ' active' : ''}`}
          onClick={() => setActiveTab('reservations')}
        >
          Reservations
        </button>
      </div>

      <div className="card">
        {/* each component only renders when its tab is active — no unnecessary API calls */}
        {activeTab === 'checkin'      && <CheckInWizard />}
        {activeTab === 'checkout'     && <CheckOutForm onDone={() => setActiveTab('checkin')} />}
        {activeTab === 'reservations' && <ReservationList />}
      </div>
    </div>
  );
}
