import { create } from 'zustand';
import { CartItem } from '../types/cart';
import { MenuItem } from '../types/menu';

interface CartState {
  items: CartItem[];
  addItem: (menuItem: MenuItem, note?: string) => void;
  removeItem: (menuItemId: string) => void;
  updateQuantity: (menuItemId: string, quantity: number) => void;
  updateNote: (menuItemId: string, note: string) => void;
  clearCart: () => void;
  getTotalPrice: () => number;
  getTotalItems: () => number;
}

export const useCartStore = create<CartState>((set, get) => ({
  items: [],

  // Increment quantity if the item is already in the cart; add a new entry otherwise.
  addItem: (menuItem, note = '') => {
    const existing = get().items.find((i) => i.menuItem.id === menuItem.id);
    if (existing) {
      set({
        items: get().items.map((i) =>
          i.menuItem.id === menuItem.id ? { ...i, quantity: i.quantity + 1 } : i
        ),
      });
    } else {
      set({ items: [...get().items, { menuItem, quantity: 1, note }] });
    }
  },

  removeItem: (menuItemId) => {
    set({ items: get().items.filter((i) => i.menuItem.id !== menuItemId) });
  },

  updateQuantity: (menuItemId, quantity) => {
    // Treat zero or negative as a remove so the UI swipe-to-delete and stepper
    // can both call the same action without branching.
    if (quantity <= 0) {
      get().removeItem(menuItemId);
      return;
    }
    set({
      items: get().items.map((i) =>
        i.menuItem.id === menuItemId ? { ...i, quantity } : i
      ),
    });
  },

  updateNote: (menuItemId, note) => {
    set({
      items: get().items.map((i) =>
        i.menuItem.id === menuItemId ? { ...i, note } : i
      ),
    });
  },

  clearCart: () => set({ items: [] }),

  getTotalPrice: () =>
    get().items.reduce((sum, i) => sum + i.menuItem.price * i.quantity, 0),

  getTotalItems: () =>
    get().items.reduce((sum, i) => sum + i.quantity, 0),
}));
