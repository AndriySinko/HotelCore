export interface User {
  id: string;
  name: string;
  roomNumber: string | null;
}

export interface AuthSession {
  user: User;
  token: string;
}
