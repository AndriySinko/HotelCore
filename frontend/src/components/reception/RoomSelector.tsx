






import { useState, useEffect } from 'react';
import { getAvailableRooms } from '../../api/receptionApi';
import type { RoomDto } from '../../types';

interface Props {
  selectedRoomId: string;
  onChange: (roomId: string) => void;
  excludeRoomId?: string;
}

export default function RoomSelector({ selectedRoomId, onChange, excludeRoomId }: Props) {
  const [rooms, setRooms] = useState<RoomDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const today    = new Date().toISOString().split('T')[0];
    const tomorrow = new Date(Date.now() + 86400000).toISOString().split('T')[0];
    getAvailableRooms(today, tomorrow)
      .then((allRooms) => {
        const filtered = excludeRoomId
          ? allRooms.filter((room) => room.id !== excludeRoomId)
          : allRooms;
        setRooms(filtered);
      })
      .catch((err: unknown) => {
        setError(err instanceof Error ? err.message : 'Failed to load rooms');
      })
      .finally(() => setLoading(false));
  }, [excludeRoomId]);

  if (loading) return <p className="body-text">Loading available rooms…</p>;
  if (error)   return <div className="alert alert-error"><span className="alert-icon">⚠</span><span>{error}</span></div>;

  if (rooms.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">🚫</div>
        <div className="empty-title">No rooms available</div>
        <div className="empty-desc">There are no available rooms at this time.</div>
      </div>
    );
  }

  return (
    <div className="alternatives-section">
      {rooms.map((room) => (
        <label
          key={room.id}
          className={`room-option${selectedRoomId === room.id ? ' selected' : ''}`}
        >
          <input
            type="radio"
            name="room"
            value={room.id}
            checked={selectedRoomId === room.id}
            onChange={() => onChange(room.id)}
          />
          <div className="room-option-body">
            <span className="room-number">Room {room.roomNumber}</span>
            <span className="room-type">{room.roomType}</span>
            <span className="room-floor">Floor {room.floor}</span>
            <span className="room-price">€{room.pricePerNight.toFixed(2)}/night</span>
          </div>
        </label>
      ))}
    </div>
  );
}
