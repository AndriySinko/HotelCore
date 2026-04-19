import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getReservation } from '../../api';
import AppShell from '../../components/AppShell';
import Stepper from '../../components/Stepper';
import { Button, Card, ReservationInfo } from '../../components/UI';
import useWizardStore from '../../store/wizardStore';

export default function Step1RoomSelection() {
  const navigate = useNavigate();
  const [showValidation, setShowValidation] = useState(false);
  const selectedRooms = useWizardStore((state) => state.selectedRooms);
  const setSelectedRooms = useWizardStore((state) => state.setSelectedRooms);

  const { data: reservation, isLoading } = useQuery({
    queryKey: ['reservation'],
    queryFn: getReservation,
  });

  const toggleRoom = (room: string) => {
    setSelectedRooms(
      selectedRooms.includes(room) ? selectedRooms.filter((selectedRoom) => selectedRoom !== room) : [...selectedRooms, room],
    );
  };

  const handleContinue = () => {
    setShowValidation(true);

    if (selectedRooms.length === 0) {
      return;
    }

    navigate('/cleaning/step/2');
  };

  const rooms = reservation?.rooms ?? [];

  return (
    <AppShell>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-semibold tracking-tight text-slate-900">Request Cleaning Services</h1>
          <p className="mt-2 text-slate-500">UC07 - Request Cleaning Services</p>
        </div>

        <Stepper currentStep={1} />

        <section className="rounded border border-slate-200 bg-white p-8 shadow-sm">
          <div className="mb-6">
            <h2 className="text-xl font-semibold text-slate-900">Step 1: Room Selection</h2>
            <p className="mt-2 text-slate-500">Select the rooms you'd like to have cleaned</p>
          </div>

          <div className="space-y-5">
            <ReservationInfo
              guestName={reservation?.guestName ?? 'Loading...'}
              roomType={reservation?.roomType ?? '--'}
              roomNumber={reservation?.roomNumber ?? '--'}
              confirmation={reservation?.confirmation ?? '--'}
              checkOut={reservation?.checkOut ?? '--'}
            />

            {isLoading ? <p className="text-sm text-slate-500">Loading available rooms...</p> : null}

            <div className="space-y-4">
              {rooms.map((room) => {
                const isSelected = selectedRooms.includes(room.room);
                return (
                  <button key={room.room} type="button" className="block w-full text-left" onClick={() => toggleRoom(room.room)}>
                    <Card
                      className={`p-5 ${isSelected ? 'border-blue-500 ring-1 ring-blue-500' : ''} ${room.muted ? 'opacity-60' : ''}`}
                    >
                      <div className="flex items-start gap-4">
                        <div
                          className={`mt-1 h-5 w-5 rounded border ${isSelected ? 'border-blue-600 bg-blue-600' : 'border-slate-300 bg-slate-50'}`}
                        />
                        <div className="min-w-0 flex-1">
                          <div className="flex items-center justify-between gap-4">
                            <h3 className={`text-lg font-medium ${room.muted ? 'text-slate-500' : 'text-slate-900'}`}>{room.room}</h3>
                            {room.badge ? (
                              <span className="rounded bg-slate-100 px-3 py-1 text-xs font-medium text-slate-500">{room.badge}</span>
                            ) : null}
                          </div>
                          <p className="mt-3 text-sm text-slate-600">Room Type: {room.type}</p>
                          <p className="mt-1 text-sm text-slate-600">Check-out: {room.checkOut}</p>
                          {room.muted ? (
                            <p className="mt-3 text-sm italic text-slate-400">
                              This room has an existing cleaning request. Click to reschedule or merge.
                            </p>
                          ) : null}
                        </div>
                      </div>
                    </Card>
                  </button>
                );
              })}
            </div>

            {showValidation && selectedRooms.length === 0 ? (
              <p className="text-sm text-red-600">Select at least one room to continue.</p>
            ) : null}

            <a href="#" className="inline-block text-sm font-medium text-slate-700 underline underline-offset-4">
              View Alternative Rooms
            </a>
          </div>
        </section>

        <div className="flex flex-col gap-4 pt-2 sm:flex-row">
          <Button variant="secondary" className="sm:w-24" onClick={() => navigate(-1)}>
            Back
          </Button>
          <Button className="flex-1" onClick={handleContinue}>
            ◎ Confirm Room Assignment
          </Button>
        </div>
      </div>
    </AppShell>
  );
}