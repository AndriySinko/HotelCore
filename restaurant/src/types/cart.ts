import { MenuItem } from './menu';

export interface CartItem {
  menuItem: MenuItem;
  quantity: number;
  note: string;
}
