export interface Passenger {
  name: string;
  age: number;
  gender: string;
}

export interface BookingRequest {
  trainId: number;
  journeyDate: string;
  numberOfPassengers: number;
  passengers: Passenger[];
}

export interface Booking {
  id?: number;
  reservationId?: number;
  pnr: string;
  trainName: string;
  trainNumber: string;
  source: string;
  destination: string;
  journeyDate: string;
  bookingDate: string;
  status: string;
  totalFare: number;
  passengers: Passenger[];
}