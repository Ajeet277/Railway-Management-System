import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Booking } from '../../services/admin';

@Component({
  selector: 'app-admin-bookings',
  imports: [CommonModule],
  templateUrl: './admin-bookings.html',
  styleUrl: './admin-bookings.css'
})
export class AdminBookingsComponent implements OnInit {
  bookings: Booking[] = [];
  filteredBookings: Booking[] = [];
  loading = false;
  searchTerm = '';
  statusFilter = '';

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.loading = true;
    this.adminService.getAllBookings().subscribe({
      next: (bookings) => {
        console.log('Loaded bookings:', bookings);
        // Sort by latest first in case backend doesn't sort
        this.bookings = bookings.sort((a, b) => 
          new Date(b.journeyDate).getTime() - new Date(a.journeyDate).getTime()
        );
        this.filteredBookings = this.bookings;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading bookings:', error);
        this.loading = false;
      }
    });
  }

  onSearch(event: any) {
    this.searchTerm = event.target.value.toLowerCase();
    this.applyFilters();
  }

  onStatusFilter(event: any) {
    this.statusFilter = event.target.value;
    this.applyFilters();
  }

  applyFilters() {
    this.filteredBookings = this.bookings.filter(booking => {
      const matchesSearch = !this.searchTerm || 
        booking.pnr.toLowerCase().includes(this.searchTerm) ||
        booking.userName.toLowerCase().includes(this.searchTerm) ||
        booking.trainName.toLowerCase().includes(this.searchTerm);
      
      const matchesStatus = !this.statusFilter || this.getStatusString(booking.status) === this.statusFilter;
      
      return matchesSearch && matchesStatus;
    });
  }

  getStatusString(status: any): string {
    if (typeof status === 'string') return status;
    
    // Handle enum values
    switch(Number(status)) {
      case 0: return 'PendingPayment';
      case 1: return 'Confirmed';
      case 2: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getStatusClass(status: any): string {
    return 'status-' + this.getStatusString(status).toLowerCase();
  }

  getConfirmedCount(): number {
    return this.filteredBookings.filter(b => this.getStatusString(b.status) === 'Confirmed').length;
  }
  
  getPendingCount(): number {
    return this.filteredBookings.filter(b => this.getStatusString(b.status) === 'PendingPayment').length;
  }
  
  getStatusIcon(status: any): string {
    const statusStr = this.getStatusString(status);
    switch(statusStr) {
      case 'Confirmed': return '✅';
      case 'PendingPayment': return '⏳';
      case 'Cancelled': return '❌';
      default: return '❓';
    }
  }

  get totalRevenue() {
    return this.filteredBookings
      .filter(b => this.getStatusString(b.status) === 'Confirmed')
      .reduce((sum, b) => sum + b.totalFare, 0);
  }
}