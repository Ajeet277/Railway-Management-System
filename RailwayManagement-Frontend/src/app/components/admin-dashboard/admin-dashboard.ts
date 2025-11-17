import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminService, Train, Booking } from '../../services/admin';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent implements OnInit {
  trains: Train[] = [];
  bookings: Booking[] = [];
  loading = false;

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    
    this.adminService.getAllTrains().subscribe({
      next: (trains) => this.trains = trains,
      error: (error) => console.error('Error loading trains:', error)
    });

    this.adminService.getAllBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading bookings:', error);
        this.loading = false;
      }
    });
  }

  get totalTrains() { return this.trains.length; }
  get confirmedBookings() { 
    return this.bookings.filter(b => {
      // Handle enum value (1 = Confirmed) or string
      return Number(b.status) === 1 || String(b.status).toLowerCase() === 'confirmed';
    }).length; 
  }
}