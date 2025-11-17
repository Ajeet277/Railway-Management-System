import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TrainService } from '../../services/train';
import { BookingService } from '../../services/booking';
import { Train } from '../../models/train.model';
import { BookingRequest } from '../../models/booking.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-booking',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css',
})
export class BookingComponent implements OnInit {
  bookingForm: FormGroup;
  train: Train | null = null;
  loading = false;
  error = '';
  minDate = new Date().toISOString().split('T')[0];

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private trainService: TrainService,
    private bookingService: BookingService
  ) {
    this.bookingForm = this.formBuilder.group({
      journeyDate: ['', Validators.required],
      passengers: this.formBuilder.array([this.createPassenger()])
    });
  }

  ngOnInit() {
    const trainId = Number(this.route.snapshot.paramMap.get('trainId'));
    const journeyDate = this.route.snapshot.queryParamMap.get('date');
    
    if (journeyDate) {
      this.bookingForm.patchValue({ journeyDate });
    }
    
    this.loadTrain(trainId);
  }

  get passengers() {
    return this.bookingForm.get('passengers') as FormArray;
  }

  createPassenger(): FormGroup {
    return this.formBuilder.group({
      name: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]+$/), Validators.minLength(2), Validators.maxLength(50)]],
      age: ['', [Validators.required, Validators.min(1), Validators.max(120)]],
      gender: ['', Validators.required]
    });
  }

  addPassenger() {
    if (this.passengers.length < 6) {
      this.passengers.push(this.createPassenger());
    }
  }

  removePassenger(index: number) {
    if (this.passengers.length > 1) {
      this.passengers.removeAt(index);
    }
  }

  loadTrain(trainId: number) {
    this.trainService.getTrain(trainId).subscribe({
      next: (train) => this.train = train,
      error: () => this.router.navigate(['/train-search'])
    });
  }

  onSubmit() {
    if (this.bookingForm.invalid || !this.train) return;

    this.loading = true;
    this.error = '';

    const bookingData: BookingRequest = {
      trainId: this.train.id,
      journeyDate: this.bookingForm.value.journeyDate,
      numberOfPassengers: this.passengers.length,
      passengers: this.bookingForm.value.passengers
    };

    this.bookingService.createBooking(bookingData).subscribe({
      next: (response) => {
        this.loading = false;
        console.log('Booking response:', response);
        if (response?.reservationId) {
          this.router.navigate(['/payment', response.reservationId]);
        } else {
          this.error = 'Booking failed: No reservation ID returned';
        }
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Booking failed';
      }
    });
  }

  getTotalFare(): number {
    return this.train ? this.train.fare * this.passengers.length : 0;
  }

  getMinDate(): string {
    return new Date().toISOString().split('T')[0];
  }
}
