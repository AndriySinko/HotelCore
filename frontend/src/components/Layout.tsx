






import { Outlet } from 'react-router-dom';
import Navbar from './Navbar';
import Toast from './Toast';

export default function Layout() {
  return (
    <div className="app-layout">
      <Navbar />
      <main className="app-content">
        <Outlet />
      </main>
      <Toast />
    </div>
  );
}
