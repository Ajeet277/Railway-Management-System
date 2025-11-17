export interface PaymentRequest {
  reservationId: number;
  paymentMethod: string;
  cardNumber?: string;
  cardHolderName?: string;
  expiryDate?: string;
  cvv?: string;
  upiId?: string;
  bankCode?: string;
  accountNumber?: string;
}

export interface PaymentResponse {
  transactionId: string;
  status: string;
  amount: number;
  paymentMethod: string;
  message: string;
}