import { useNavigate } from 'react-router-dom';
import AppShell from '../../components/AppShell';
import Stepper from '../../components/Stepper';
import { Button, Card, Input, RadioCard, ReservationInfo } from '../../components/UI';
import useWizardStore from '../../store/wizardStore';

const charges = [
  { label: 'Cleaning Service (1 room)', amount: '$25.00' },
  { label: 'Immediate Service Surcharge', amount: '$10.00' },
  { label: 'Tax (0%)', amount: '$0.00' },
];

export default function Step3Payment() {
  const navigate = useNavigate();
  const paymentMethod = useWizardStore((state) => state.paymentMethod);
  const setPaymentMethod = useWizardStore((state) => state.setPaymentMethod);

  return (
    <AppShell>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-semibold tracking-tight text-slate-900">Request Cleaning Services</h1>
          <p className="mt-2 text-slate-500">UC07 - Request Cleaning Services</p>
        </div>

        <Stepper currentStep={3} />

        <section className="rounded border border-slate-200 bg-white p-8 shadow-sm">
          <div className="mb-6">
            <h2 className="text-xl font-semibold text-slate-900">Step 3: Payment Summary</h2>
            <p className="mt-2 text-slate-500">Review charges and proceed to payment processing</p>
          </div>

          <div className="space-y-6">
            <ReservationInfo
              guestName="John Smith"
              roomType="Suite"
              roomNumber="305"
              confirmation="RES492"
              checkOut="2026-03-25"
            />

            <div>
              <h3 className="text-base font-medium text-slate-900">Charge Summary</h3>
              <div className="mt-4 space-y-0 divide-y divide-slate-200 rounded border border-slate-200 bg-white">
                {charges.map((charge) => (
                  <div key={charge.label} className="flex items-center justify-between px-4 py-4 text-sm">
                    <span className="text-slate-600">{charge.label}</span>
                    <span className="font-medium text-slate-900">{charge.amount}</span>
                  </div>
                ))}
              </div>

              <div className="mt-4 border-y-2 border-slate-800 py-4 text-base">
                <div className="flex items-center justify-between">
                  <span className="font-medium text-slate-900">Total Amount</span>
                  <span className="text-2xl font-semibold text-slate-900">$35.00</span>
                </div>
              </div>

              <div className="mt-8 flex items-center justify-between bg-slate-900 px-4 py-3 text-white">
                <span className="text-sm">Amount Due</span>
                <span className="text-2xl font-medium">$35.00</span>
              </div>
            </div>

            <Card className="border-sky-200 bg-sky-50 p-4">
              <p className="text-sm text-blue-700">
                ℹ Payment will be processed upon confirmation. Please verify all information is correct before proceeding.
              </p>
            </Card>

            <div>
              <h3 className="mb-3 text-base font-medium text-slate-900">Select Payment Method</h3>
              <div className="grid gap-3 lg:grid-cols-2">
                <button type="button" className="text-left" onClick={() => setPaymentMethod('card')}>
                  <RadioCard title="Credit / Debit Card" description="Pay with Visa, Mastercard, or AMEX" selected={paymentMethod === 'card'} />
                </button>
                <button type="button" className="text-left" onClick={() => setPaymentMethod('cash')}>
                  <RadioCard title="Cash" description="Pay at the front desk" selected={paymentMethod === 'cash'} />
                </button>
              </div>
            </div>

            {paymentMethod === 'card' ? (
              <Card className="p-6">
                <h3 className="text-base font-medium text-slate-900">Card Details</h3>
                <div className="mt-5 space-y-4">
                  <div>
                    <label className="mb-2 block text-sm text-slate-700">Cardholder Name</label>
                    <Input placeholder="e.g. John Smith" />
                  </div>
                  <div>
                    <label className="mb-2 block text-sm text-slate-700">Card Number</label>
                    <Input placeholder="0000 0000 0000 0000" />
                  </div>
                  <div className="grid gap-4 md:grid-cols-2">
                    <div>
                      <label className="mb-2 block text-sm text-slate-700">Expiry Date</label>
                      <Input placeholder="MM/YY" />
                    </div>
                    <div>
                      <label className="mb-2 block text-sm text-slate-700">CVV</label>
                      <Input placeholder="000" />
                    </div>
                  </div>
                </div>
              </Card>
            ) : null}
          </div>
        </section>

        <div className="flex flex-col gap-4 pt-2 sm:flex-row">
          <Button variant="secondary" className="sm:w-24" onClick={() => navigate('/cleaning/step/2')}>
            Back
          </Button>
          <Button className="flex-1" onClick={() => navigate('/cleaning/step/4')}>
            ◎ Proceed to Payment
          </Button>
        </div>
      </div>
    </AppShell>
  );
}