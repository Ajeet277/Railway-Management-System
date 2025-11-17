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

export interface TrainSearchRequest {
  source: string;
  destination: string;
  date: string;
}