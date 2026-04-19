import Card from './Card';

type ReservationInfoProps = {
  guestName: string;
  roomType: string;
  roomNumber: string;
  confirmation: string;
  checkOut: string;
};

export default function ReservationInfo({ guestName, roomType, roomNumber, confirmation, checkOut }: ReservationInfoProps) {
  return (
    <Card className="bg-sky-50 p-6">
      <h3 className="mb-4 text-base font-semibold text-slate-900">Reservation Details</h3>
      <div className="grid gap-4 text-sm sm:grid-cols-2">
        <div>
          <span className="font-medium text-slate-800">Guest: </span>
          <span className="text-slate-700">{guestName}</span>
        </div>
        <div>
          <span className="font-medium text-slate-800">Rooms: </span>
          <span className="text-slate-700">{roomNumber}</span>
        </div>
        <div>
          <span className="font-medium text-slate-800">Room Type: </span>
          <span className="text-slate-700">{roomType}</span>
        </div>
        <div>
          <span className="font-medium text-slate-800">Check-out: </span>
          <span className="text-slate-700">{checkOut}</span>
        </div>
        <div>
          <span className="font-medium text-slate-800">Confirmation: </span>
          <span className="text-slate-700">{confirmation}</span>
        </div>
      </div>
    </Card>
  );
}