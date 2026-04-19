import { useNavigate } from 'react-router-dom';
import AppShell from '../../components/AppShell';
import Stepper from '../../components/Stepper';
import { Button, Card, Input, RadioCard, ReservationInfo } from '../../components/UI';
import useWizardStore from '../../store/wizardStore';

const timeSlots = ['09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00'];

export default function Step2Schedule() {
  const navigate = useNavigate();
  const cleaningMode = useWizardStore((state) => state.cleaningMode);
  const scheduledDate = useWizardStore((state) => state.scheduledDate);
  const scheduledTime = useWizardStore((state) => state.scheduledTime);
  const setCleaningMode = useWizardStore((state) => state.setCleaningMode);
  const setScheduledDate = useWizardStore((state) => state.setScheduledDate);
  const setScheduledTime = useWizardStore((state) => state.setScheduledTime);

  return (
    <AppShell>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-semibold tracking-tight text-slate-900">Request Cleaning Services</h1>
          <p className="mt-2 text-slate-500">UC07 - Request Cleaning Services</p>
        </div>

        <Stepper currentStep={2} />

        <section className="rounded border border-slate-200 bg-white p-8 shadow-sm">
          <div className="mb-6">
            <h2 className="text-xl font-semibold text-slate-900">Step 2: Cleaning Type Selection</h2>
            <p className="mt-2 text-slate-500">Choose when you'd like the cleaning to be performed</p>
          </div>

          <div className="space-y-6">
            <ReservationInfo
              guestName="John Smith"
              roomType="Deluxe Double"
              roomNumber="305"
              confirmation="RES492"
              checkOut="2026-03-25"
            />

            <div className="grid gap-3 lg:grid-cols-2">
              <button type="button" className="h-full text-left" onClick={() => setCleaningMode('immediate')}>
                <RadioCard
                  className="h-full"
                  title="Immediate Cleaning"
                  description="Request cleaning to start as soon as possible. Staff will be assigned immediately."
                  selected={cleaningMode === 'immediate'}
                >
                  <div className="inline-flex rounded border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700">
                    ◎ Ready for check-in
                  </div>
                  <p className="mt-3 text-sm text-slate-500">Estimated start time: Within 30 minutes</p>
                </RadioCard>
              </button>

              <button type="button" className="h-full text-left" onClick={() => setCleaningMode('scheduled')}>
                <RadioCard
                  className="h-full"
                  title="Scheduled Cleaning"
                  description="Schedule cleaning for a specific date and time that works best for you."
                  selected={cleaningMode === 'scheduled'}
                />
              </button>
            </div>

            {cleaningMode === 'scheduled' ? (
              <Card className="p-6">
                <h3 className="text-base font-medium text-slate-900">Select Date and Time</h3>
                <p className="mt-2 text-sm text-slate-500">Please select date and time for scheduled cleaning.</p>
                <div className="mt-5 grid gap-6 lg:grid-cols-2">
                  <div>
                    <label className="mb-3 block text-sm font-medium text-slate-700">Date</label>
                    <Input value={scheduledDate ?? ''} placeholder="YYYY-MM-DD" onChange={(event) => setScheduledDate(event.target.value || null)} />
                  </div>
                  <div>
                    <label className="mb-3 block text-sm font-medium text-slate-700">Time Slot</label>
                    <div className="grid grid-cols-3 gap-2">
                      {timeSlots.map((slot) => {
                        const isSelected = scheduledTime === slot;
                        return (
                          <button
                            key={slot}
                            type="button"
                            onClick={() => setScheduledTime(slot)}
                            className={`h-10 rounded border text-sm text-slate-900 ${isSelected ? 'border-blue-500 bg-slate-50' : 'border-slate-200 bg-white'}`}
                          >
                            {slot}
                          </button>
                        );
                      })}
                    </div>
                  </div>
                </div>
              </Card>
            ) : null}
          </div>
        </section>

        <div className="flex flex-col gap-4 pt-2 sm:flex-row">
          <Button variant="secondary" className="sm:w-24" onClick={() => navigate('/cleaning/step/1')}>
            Back
          </Button>
          <Button className="flex-1" onClick={() => navigate('/cleaning/step/3')}>
            Continue to Payment
          </Button>
        </div>
      </div>
    </AppShell>
  );
}