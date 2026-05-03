import type { ReservationDto, RoomDto } from '../../../types/checkIn';
import Alert from '../../Alert';

interface Props {
  reservation: ReservationDto;
  onRoomConfirmed: (room: RoomDto) => void;
  onCancel: () => void;
}

export default function RoomStatus({ reservation, onRoomConfirmed, onCancel }: Props) {
  const room = reservation.room;
  const isReady = room.status === 'Reserved' || room.status === 'Available';

  if (!isReady) {
    return (
      <div className="step-content">
        <h2 className="step-title">Room Not Ready</h2>
        <Alert
          type="error"
          message={`Room ${room.number} is currently ${room.status.toLowerCase()}. Please contact housekeeping or try again later.`}
        />
        <div className="btn-row">
          <button className="btn btn-ghost" onClick={onCancel}>Cancel Check-In</button>
        </div>
      </div>
    );
  }

  return (
    <div className="step-content">
      <h2 className="step-title">Room Status</h2>
      <Alert type="success" message={`Room ${room.number} is ready for check-in.`} />

      <div className="info-card" style={{ marginTop: '1.25rem' }}>
        <div className="info-row">
          <span className="info-label">Room Number</span>
          <span className="info-value room-number-lg">{room.number}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Type</span>
          <span className="info-value">{room.roomType.name}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Floor</span>
          <span className="info-value">{room.floor}</span>
        </div>
        <div className="info-row">
          <span className="info-label">Rate</span>
          <span className="info-value">${room.roomType.pricePerNight.toFixed(2)} / night</span>
        </div>
        <div className="info-row">
          <span className="info-label">Status</span>
          <span className="info-value"><span className="badge badge-available">Ready</span></span>
        </div>
      </div>

      <div className="btn-row">
        <button className="btn btn-primary" onClick={() => onRoomConfirmed(room)}>
          Confirm Room & Continue
        </button>
        <button className="btn btn-ghost" onClick={onCancel}>Back</button>
      </div>
    </div>
  );
}
