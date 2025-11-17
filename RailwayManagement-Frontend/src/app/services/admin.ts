import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Train {
  id: number;
  trainNumber: string;
  trainName: string;
  source: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  totalSeats: number;
  availableSeats: number;
  fare: number;
  class: string;
}

export interface AddTrainRequest {
  trainNumber: string;
  trainName: string;
  source: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  totalSeats: number;
  fare: number;
  class: string;
}

export interface UpdateTrainRequest {
  trainName?: string;
  source?: string;
  destination?: string;
  departureTime?: string;
  arrivalTime?: string;
  totalSeats?: number;
  fare?: number;
  class?: string;
}

export interface Booking {
  pnr: string;
  userName: string;
  userEmail: string;
  trainName: string;
  journeyDate: string;
  totalFare: number;
  status: string;
  passengerCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  constructor(private http: HttpClient) {}

  // Train Management
  getAllTrains(): Observable<Train[]> {
    return this.http.get<Train[]>(`${environment.apiUrl}/train`);
  }

  addTrain(train: AddTrainRequest): Observable<Train> {
    return this.http.post<Train>(`${environment.apiUrl}/admin/trains`, train);
  }

  updateTrain(id: number, train: UpdateTrainRequest): Observable<Train> {
    return this.http.put<Train>(`${environment.apiUrl}/admin/trains/${id}`, train, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  deleteTrain(id: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/admin/trains/${id}`);
  }

  // Booking Management
  getAllBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${environment.apiUrl}/admin/bookings`);
  }
}