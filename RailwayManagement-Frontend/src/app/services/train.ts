import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Train, TrainSearchRequest } from '../models/train.model';

@Injectable({
  providedIn: 'root'
})
export class TrainService {
  constructor(private http: HttpClient) {}

  searchTrains(request: TrainSearchRequest): Observable<Train[]> {
    const params = `source=${request.source}&destination=${request.destination}&date=${request.date}`;
    return this.http.get<Train[]>(`${environment.apiUrl}/train/search?${params}`);
  }

  getTrain(id: number): Observable<Train> {
    return this.http.get<Train>(`${environment.apiUrl}/train/${id}`);
  }

  getAllTrains(): Observable<Train[]> {
    return this.http.get<Train[]>(`${environment.apiUrl}/train`);
  }

  searchTrainByNumber(trainNumber: string): Observable<Train[]> {
    return this.http.get<Train[]>(`${environment.apiUrl}/train/number/${trainNumber}`);
  }
}
