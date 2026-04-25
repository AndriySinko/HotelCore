import { apiClient } from './apiClient';
import { User } from '../types/auth';

export interface QrLoginResponse {
  token: string;
  guestId: string;
  name: string;
  roomNumber: string | null;
}

export const authApi = {
  async loginWithQrCode(qrData: string): Promise<{ user: User; token: string }> {
    const res = await apiClient.post<QrLoginResponse>('/restaurant/auth/qr', {
      qrToken: qrData,
    });
    const data = res.data;
    return {
      token: data.token,
      user: {
        id: data.guestId,
        name: data.name,
        roomNumber: data.roomNumber,
      },
    };
  },

  async logout(): Promise<void> {
    // JWT is stateless; just clear local state
  },
};
