// Values match the snake_case strings returned by the backend's MapStatus switch.
export type OrderStatus =
  | 'received'
  | 'preparing'
  | 'on_the_way'
  | 'delivered'
  | 'cancelled'
  | 'in_progress';

// Values match the snake_case strings returned by MapPaymentMethod on the backend.
export type PaymentMethod = 'room_bill' | 'online_payment';

export const ORDER_STATUS_LABEL: Record<OrderStatus, string> = {
  in_progress: 'In Progress',
  received:    'Received',
  preparing:   'Preparing',
  on_the_way:  'On the Way',
  delivered:   'Delivered',
  cancelled:   'Cancelled',
};

export interface OrderItem {
  id: string;
  productName: string;
  pricePerUnit: number;
  quantity: number;
  specialRequest?: string | null;
}

export interface Order {
  id: string;
  status: OrderStatus;
  items: OrderItem[];
  totalPrice: number;
  paymentMethod: PaymentMethod;
  createdAt: string;
}
