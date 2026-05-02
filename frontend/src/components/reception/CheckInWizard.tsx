import { useState } from 'react';
import type { CheckInState, ReservationDto, RoomDto, IdentityData, PaymentMethod } from '../../types/checkIn';
import StepIndicator from '../StepIndicator';
import SearchReservation from './steps/SearchReservation';
import VerifyIdentity from './steps/VerifyIdentity';
import RoomStatus from './steps/RoomStatus';
import Payment from './steps/Payment';
import Confirmation from './steps/Confirmation';

const INITIAL_STATE: CheckInState = {
  step: 'search',
  reservation: null,
  identityData: null,
  selectedRoom: null,
  paymentMethod: null,
  result: null,
};

export default function CheckInWizard() {
  const [state, setState] = useState<CheckInState>(INITIAL_STATE);

  const { step, reservation, identityData, selectedRoom, paymentMethod } = state;

  function onReservationSelected(r: ReservationDto) {
    setState(s => ({ ...s, reservation: r, step: 'identity' }));
  }

  function onIdentityVerified(data: IdentityData) {
    setState(s => ({ ...s, identityData: data, step: 'room' }));
  }

  function onRoomConfirmed(room: RoomDto) {
    setState(s => ({ ...s, selectedRoom: room, step: 'payment' }));
  }

  function onPaymentComplete(method: PaymentMethod) {
    setState(s => ({ ...s, paymentMethod: method, step: 'confirmation' }));
  }

  function handleBack() {
    if (step === 'identity')     setState(s => ({ ...s, step: 'search' }));
    if (step === 'room')         setState(s => ({ ...s, step: 'identity' }));
    if (step === 'payment')      setState(s => ({ ...s, step: 'room' }));
  }

  const backLabel: Partial<Record<typeof step, string>> = {
    identity:    '← Find Reservation',
    room:        '← Verify Identity',
    payment:     '← Room Status',
  };

  return (
    <div className="wizard-container">
      <StepIndicator current={step} />

      {backLabel[step] && (
        <button className="step-back-btn" onClick={handleBack}>{backLabel[step]}</button>
      )}

      <div className="wizard-body">
        {step === 'search' && (
          <SearchReservation onReservationSelected={onReservationSelected} />
        )}

        {step === 'identity' && reservation && (
          <VerifyIdentity
            reservation={reservation}
            onVerified={onIdentityVerified}
            onCancel={() => setState(s => ({ ...s, step: 'search' }))}
          />
        )}

        {step === 'room' && reservation && (
          <RoomStatus
            reservation={reservation}
            onRoomConfirmed={onRoomConfirmed}
            onCancel={() => setState(s => ({ ...s, step: 'identity' }))}
          />
        )}

        {step === 'payment' && reservation && selectedRoom && (
          <Payment
            reservation={reservation}
            selectedRoom={selectedRoom}
            onPaymentComplete={onPaymentComplete}
            onCancel={() => setState(s => ({ ...s, step: 'room' }))}
          />
        )}

        {step === 'confirmation' && reservation && selectedRoom && identityData && paymentMethod && (
          <Confirmation
            reservation={reservation}
            selectedRoom={selectedRoom}
            identityData={identityData}
            paymentMethod={paymentMethod}
            onNewCheckIn={() => setState(INITIAL_STATE)}
          />
        )}
      </div>
    </div>
  );
}
