import { create } from 'zustand';
import { Order, PaymentMethod } from '../types/order';
import { CartItem } from '../types/cart';
import { orderApi } from '../api/orderApi';

interface OrderState {
  activeOrder: Order | null;
  orderHistory: Order[];
  ordersById: Record<string, Order>;
  isLoading: boolean;
  error: string | null;
  placeOrder: (cartItems: CartItem[], paymentMethod: PaymentMethod) => Promise<Order>;
  cancelOrder: (orderId: string) => Promise<void>;
  refreshOrder: (orderId: string) => Promise<void>;
  clearOrder: () => void;
  clearError: () => void;
}

export const useOrderStore = create<OrderState>((set, get) => ({
  activeOrder: null,
  orderHistory: [],
  ordersById: {},
  isLoading: false,
  error: null,

  placeOrder: async (cartItems, paymentMethod) => {
    set({ isLoading: true, error: null });
    try {
      const order = await orderApi.createOrder(cartItems, paymentMethod);
      set({
        activeOrder: order,
        orderHistory: [order, ...get().orderHistory],
        ordersById: { ...get().ordersById, [order.id]: order },
        isLoading: false,
      });
      return order;
    } catch (e: unknown) {
      const message = e instanceof Error ? e.message : 'Failed to place order';
      set({ error: message, isLoading: false });
      throw e;
    }
  },

  cancelOrder: async (orderId) => {
    set({ isLoading: true, error: null });
    try {
      const order = await orderApi.cancelOrder(orderId);
      set({
        activeOrder: order,
        ordersById: { ...get().ordersById, [order.id]: order },
        orderHistory: get().orderHistory.map((o) => (o.id === order.id ? order : o)),
        isLoading: false,
      });
    } catch (e: unknown) {
      const message = e instanceof Error ? e.message : 'Failed to cancel order';
      set({ error: message, isLoading: false });
      throw e;
    }
  },

  refreshOrder: async (orderId) => {
    try {
      const order = await orderApi.getOrder(orderId);
      set({
        activeOrder: order,
        ordersById: { ...get().ordersById, [order.id]: order },
        orderHistory: get().orderHistory.map((o) => (o.id === order.id ? order : o)),
      });
    } catch {
      // silently ignore refresh errors
    }
  },

  clearOrder: () => set({ activeOrder: null }),
  clearError: () => set({ error: null }),
}));
