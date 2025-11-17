import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BookingRequest, Booking } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  constructor(private http: HttpClient) {}

  createBooking(request: BookingRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/reservation`, request);
  }

  getMyBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${environment.apiUrl}/reservation/my-bookings`);
  }

  getBookingByPNR(pnr: string): Observable<Booking> {
    return this.http.get<Booking>(`${environment.apiUrl}/reservation/pnr/${pnr}`);
  }

  cancelBooking(pnr: string, reason: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/cancellation/${pnr}`, JSON.stringify(reason), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
