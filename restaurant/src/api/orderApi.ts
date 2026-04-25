import { apiClient } from './apiClient';
import { Order, PaymentMethod } from '../types/order';
import { CartItem } from '../types/cart';

interface CreateOrderItemDto {
  productId: string;
  quantity: number;
  specialRequest?: string | null;
}

interface CreateOrderPayload {
  items: CreateOrderItemDto[];
  paymentMethod: 'RoomBill' | 'OnlinePayment';
}

function toBackendPaymentMethod(method: PaymentMethod): 'RoomBill' | 'OnlinePayment' {
  return method === 'room_bill' ? 'RoomBill' : 'OnlinePayment';
}

export const orderApi = {
  async createOrder(cartItems: CartItem[], paymentMethod: PaymentMethod): Promise<Order> {
    if (paymentMethod === 'online_payment') {
      await new Promise((r) => setTimeout(r, 1000));
    }

    const payload: CreateOrderPayload = {
      items: cartItems.map((ci) => ({
        productId: ci.menuItem.id,
        quantity: ci.quantity,
        specialRequest: ci.note || null,
      })),
      paymentMethod: toBackendPaymentMethod(paymentMethod),
    };

    const res = await apiClient.post<Order>('/restaurant/orders', payload);
    return res.data;
  },

  async getOrder(orderId: string): Promise<Order> {
    const res = await apiClient.get<Order>(`/restaurant/orders/${orderId}`);
    return res.data;
  },

  async cancelOrder(orderId: string): Promise<Order> {
    const res = await apiClient.delete<Order>(`/restaurant/orders/${orderId}`);
    return res.data;
  },
};
