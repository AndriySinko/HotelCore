import { mockResponse } from './client';

export type ProcessPaymentPayload = {
  method: 'card' | 'cash';
  amount: number;
  cardholderName?: string;
  cardNumber?: string;
};

export type ProcessPaymentResponse = {
  status: 'approved';
  token?: string;
  last4?: string;
};

export async function processPayment(payload: ProcessPaymentPayload): Promise<ProcessPaymentResponse> {
  if (payload.method === 'cash') {
    return mockResponse({ status: 'approved' });
  }

  const digits = (payload.cardNumber ?? '').replace(/\D/g, '');
  const last4 = digits.slice(-4) || '4242';

  return mockResponse({
    status: 'approved',
    token: `tok_mock_${last4}`,
    last4,
  });
}
