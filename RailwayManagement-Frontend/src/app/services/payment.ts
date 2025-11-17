import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PaymentRequest, PaymentResponse } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  constructor(private http: HttpClient) {}

  processPayment(request: PaymentRequest): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${environment.apiUrl}/payment/process`, request);
  }

  getPaymentHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/payment/history`);
  }

  getPaymentStatus(reservationId: number): Observable<any> {
    return this.http.get(`${environment.apiUrl}/payment/reservation/${reservationId}`);
  }
}
