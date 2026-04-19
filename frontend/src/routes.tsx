import { Navigate, Route, Routes } from 'react-router-dom';
import Step1RoomSelection from './pages/cleaning/Step1RoomSelection';
import Step2Schedule from './pages/cleaning/Step2Schedule';
import Step3Payment from './pages/cleaning/Step3Payment';
import Step4Complete from './pages/cleaning/Step4Complete';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/cleaning/step/1" replace />} />
      <Route path="/cleaning/step/1" element={<Step1RoomSelection />} />
      <Route path="/cleaning/step/2" element={<Step2Schedule />} />
      <Route path="/cleaning/step/3" element={<Step3Payment />} />
      <Route path="/cleaning/step/4" element={<Step4Complete />} />
    </Routes>
  );
}