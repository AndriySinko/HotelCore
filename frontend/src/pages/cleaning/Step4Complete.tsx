import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { getReservation } from '../../api';
import AppShell from '../../components/AppShell';
import Stepper from '../../components/Stepper';
import { Button, Card } from '../../components/UI';
import useWizardStore from '../../store/wizardStore';

export default function Step4Complete() {
  const navigate = useNavigate();
  const cleaningMode = useWizardStore((state) => state.cleaningMode);
  const reset = useWizardStore((state) => state.reset);
  const { data: reservation, isLoading } = useQuery({
    queryKey: ['reservation'],
    queryFn: getReservation,
  });

  const handleRestart = () => {
    reset();
    navigate('/cleaning/step/1');
  };

  return (
    <AppShell>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-semibold tracking-tight text-slate-900">Request Cleaning Services</h1>
          <p className="mt-2 text-slate-500">UC07 - Request Cleaning Services</p>
        </div>

        <Stepper currentStep={4} />

        <section className="rounded border border-slate-200 bg-white p-8 shadow-sm">
          <div className="space-y-6">
            <div className="rounded border border-emerald-200 bg-emerald-50 p-6 text-center">
              <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-emerald-500 text-2xl text-white">
                ✓
              </div>
              <h2 className="mt-4 text-2xl font-semibold text-slate-900">Cleaning Request Submitted</h2>
              <p className="mt-2 text-slate-600">Your request has been completed successfully.</p>
            </div>

            <Card className="p-6">
              <h3 className="text-base font-medium text-slate-900">Summary</h3>
              {isLoading ? <p className="mt-3 text-sm text-slate-500">Loading reservation details...</p> : null}
              <div className="mt-4 grid gap-3 text-sm text-slate-700 sm:grid-cols-2">
                <div>
                  <span className="font-medium text-slate-900">Guest: </span>
                  {reservation?.guestName ?? '--'}
                </div>
                <div>
                  <span className="font-medium text-slate-900">Room: </span>
                  {reservation?.roomNumber ?? '--'}
                </div>
                <div>
                  <span className="font-medium text-slate-900">Cleaning Mode: </span>
                  {cleaningMode === 'scheduled' ? 'Scheduled' : 'Immediate'}
                </div>
                <div>
                  <span className="font-medium text-slate-900">Amount Due: </span>$35.00
                </div>
              </div>
            </Card>
          </div>
        </section>

        <div className="flex flex-col gap-4 pt-2 sm:flex-row">
          <Button className="flex-1" onClick={handleRestart}>
            Restart Request
          </Button>
        </div>
      </div>
    </AppShell>
  );
}