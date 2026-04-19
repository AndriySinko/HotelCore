import { useMutation, useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getReservation, processPayment } from '../../api';
import AppShell from '../../components/AppShell';
import Stepper from '../../components/Stepper';
import { Button, Card, Input, RadioCard, ReservationInfo } from '../../components/UI';
import useWizardStore from '../../store/wizardStore';

const charges = [
  { label: 'Cleaning Service (1 room)', amount: '$25.00' },
  { label: 'Immediate Service Surcharge', amount: '$10.00' },
  { label: 'Tax (0%)', amount: '$0.00' },
];

function formatCardNumber(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 16);
  return digits.replace(/(.{4})/g, '$1 ').trim();
}

function formatExpiry(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 4);
  if (digits.length <= 2) {
    return digits;
  }

  return `${digits.slice(0, 2)}/${digits.slice(2)}`;
}

function formatCvv(value: string) {
  return value.replace(/\D/g, '').slice(0, 4);
}

export default function Step3Payment() {
  const navigate = useNavigate();
  const [showValidation, setShowValidation] = useState(false);
  const [cardNumber, setCardNumber] = useState('');
  const [expiryDate, setExpiryDate] = useState('');
  const [cvv, setCvv] = useState('');
  const paymentMethod = useWizardStore((state) => state.paymentMethod);
  const paymentDetails = useWizardStore((state) => state.paymentDetails);
  const setPaymentMethod = useWizardStore((state) => state.setPaymentMethod);
  const setPaymentDetails = useWizardStore((state) => state.setPaymentDetails);

  const { data: reservation, isLoading } = useQuery({
    queryKey: ['reservation'],
    queryFn: getReservation,
  });

  const processPaymentMutation = useMutation({
    mutationFn: processPayment,
  });

  const isCardNameMissing = paymentMethod === 'card' && paymentDetails.name.trim().length === 0;
  const isCardNumberMissing = paymentMethod === 'card' && cardNumber.trim().length === 0;
  const isExpiryMissing = paymentMethod === 'card' && expiryDate.trim().length === 0;
  const isCvvMissing = paymentMethod === 'card' && cvv.trim().length === 0;
  const cardDigitsCount = cardNumber.replace(/\D/g, '').length;
  const isCardNumberInvalid = paymentMethod === 'card' && cardDigitsCount > 0 && cardDigitsCount < 16;
  const isExpiryInvalid = paymentMethod === 'card' && expiryDate.length > 0 && expiryDate.length < 5;
  const isCvvInvalid = paymentMethod === 'card' && cvv.length > 0 && cvv.length < 3;
  const canProceed =
    paymentMethod === 'cash' ||
    (!isCardNameMissing && !isCardNumberMissing && !isExpiryMissing && !isCvvMissing && !isCardNumberInvalid && !isExpiryInvalid && !isCvvInvalid);

  const handleProceed = async () => {
    setShowValidation(true);

    if (!canProceed) {
      return;
    }

    const paymentResponse = await processPaymentMutation.mutateAsync({
      method: paymentMethod,
      amount: 35,
      cardholderName: paymentDetails.name,
      cardNumber,
    });

    if (paymentMethod === 'card') {
      setPaymentDetails({
        ...paymentDetails,
        token: paymentResponse.token,
        last4: paymentResponse.last4,
      });
    }

    navigate('/cleaning/step/4');
  };

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
            <div className={isLoading ? 'animate-pulse' : ''}>
              <ReservationInfo
                guestName={reservation?.guestName ?? 'Loading...'}
                roomType={reservation?.roomType ?? '--'}
                roomNumber={reservation?.roomNumber ?? '--'}
                confirmation={reservation?.confirmation ?? '--'}
                checkOut={reservation?.checkOut ?? '--'}
              />
            </div>

            {isLoading ? <p className="text-sm text-slate-500">Loading reservation details...</p> : null}

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

              <div className="mt-6 flex items-center justify-between rounded bg-slate-900 px-4 py-3 text-white">
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
                    <label className="mb-2 block text-sm text-slate-700">
                      Cardholder Name <span className="text-red-500">*</span>
                    </label>
                    <Input
                      placeholder="e.g. John Smith"
                      value={paymentDetails.name}
                      className={showValidation && isCardNameMissing ? 'border-red-300' : ''}
                      onChange={(event) => setPaymentDetails({ ...paymentDetails, name: event.target.value })}
                    />
                    {showValidation && isCardNameMissing ? <p className="mt-2 text-xs text-red-600">Cardholder name is required.</p> : null}
                  </div>
                  <div>
                    <label className="mb-2 block text-sm text-slate-700">
                      Card Number <span className="text-red-500">*</span>
                    </label>
                    <Input
                      placeholder="0000 0000 0000 0000"
                      value={cardNumber}
                      className={showValidation && isCardNumberMissing ? 'border-red-300' : ''}
                      onChange={(event) => setCardNumber(formatCardNumber(event.target.value))}
                    />
                    {showValidation && isCardNumberMissing ? <p className="mt-2 text-xs text-red-600">Card number is required.</p> : null}
                    {showValidation && !isCardNumberMissing && isCardNumberInvalid ? (
                      <p className="mt-2 text-xs text-red-600">Card number must be 16 digits.</p>
                    ) : null}
                  </div>
                  <div className="grid gap-4 md:grid-cols-2">
                    <div>
                      <label className="mb-2 block text-sm text-slate-700">
                        Expiry Date <span className="text-red-500">*</span>
                      </label>
                      <Input
                        placeholder="MM/YY"
                        value={expiryDate}
                        className={showValidation && isExpiryMissing ? 'border-red-300' : ''}
                        onChange={(event) => setExpiryDate(formatExpiry(event.target.value))}
                      />
                      {showValidation && isExpiryMissing ? <p className="mt-2 text-xs text-red-600">Expiry date is required.</p> : null}
                      {showValidation && !isExpiryMissing && isExpiryInvalid ? <p className="mt-2 text-xs text-red-600">Use MM/YY format.</p> : null}
                    </div>
                    <div>
                      <label className="mb-2 block text-sm text-slate-700">
                        CVV <span className="text-red-500">*</span>
                      </label>
                      <Input
                        placeholder="000"
                        value={cvv}
                        className={showValidation && isCvvMissing ? 'border-red-300' : ''}
                        onChange={(event) => setCvv(formatCvv(event.target.value))}
                      />
                      {showValidation && isCvvMissing ? <p className="mt-2 text-xs text-red-600">CVV is required.</p> : null}
                      {showValidation && !isCvvMissing && isCvvInvalid ? <p className="mt-2 text-xs text-red-600">CVV must be 3 or 4 digits.</p> : null}
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
          <Button className="flex-1" onClick={handleProceed} disabled={processPaymentMutation.isPending}>
            {processPaymentMutation.isPending ? 'Processing...' : '◎ Proceed to Payment'}
          </Button>
        </div>
      </div>
    </AppShell>
  );
}