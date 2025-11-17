import { Component, OnInit } from '@angular/core';
import { BookingService } from '../../services/booking';
import { PaymentService } from '../../services/payment';
import { Booking } from '../../models/booking.model';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-my-bookings',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.css',
})
export class MyBookingsComponent implements OnInit {
  bookings: Booking[] = [];
  loading = true;
  error = '';
  showCancelModal = false;
  selectedBooking: Booking | null = null;
  cancelReason = '';
  cancelling = false;

  constructor(
    private bookingService: BookingService,
    private paymentService: PaymentService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.bookingService.getMyBookings().subscribe({
      next: (bookings) => {
        // Sort by booking date (latest first) in case backend doesn't sort
        this.bookings = bookings.sort((a, b) => 
          new Date(b.bookingDate).getTime() - new Date(a.bookingDate).getTime()
        );
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load bookings';
        this.loading = false;
      }
    });
  }

  openCancelModal(booking: Booking) {
    this.selectedBooking = booking;
    this.showCancelModal = true;
    this.cancelReason = '';
  }

  closeCancelModal() {
    this.showCancelModal = false;
    this.selectedBooking = null;
    this.cancelReason = '';
    this.cancelling = false;
  }

  confirmCancellation() {
    if (!this.selectedBooking || !this.cancelReason.trim()) {
      this.showToast('Please provide a cancellation reason', 'error');
      return;
    }

    this.cancelling = true;
    this.bookingService.cancelBooking(this.selectedBooking.pnr, this.cancelReason).subscribe({
      next: () => {
        this.showToast('Booking cancelled successfully', 'success');
        this.closeCancelModal();
        this.loadBookings();
      },
      error: () => {
        this.showToast('Failed to cancel booking', 'error');
        this.cancelling = false;
      }
    });
  }

  getStatusClass(status: any): string {
    // Handle both numeric and string status values
    const statusStr = status?.toString().toLowerCase();
    
    if (status === 0 || statusStr === '0' || statusStr === 'pending' || statusStr === 'pendingpayment') {
      return 'status-pending';
    }
    if (status === 1 || statusStr === '1' || statusStr === 'confirmed') {
      return 'status-confirmed';
    }
    if (status === 2 || statusStr === '2' || statusStr === 'cancelled') {
      return 'status-cancelled';
    }
    return '';
  }

  getStatusText(status: any): string {
    if (status === 0 || status === '0') return 'Pending Payment';
    if (status === 1 || status === '1') return 'Confirmed';
    if (status === 2 || status === '2') return 'Cancelled';
    return status?.toString() || 'Unknown';
  }

  isPendingPayment(status: any): boolean {
    return status === 0 || status === '0' || status?.toString().toLowerCase() === 'pending' || status?.toString().toLowerCase() === 'pendingpayment';
  }

  isConfirmed(status: any): boolean {
    return status === 1 || status === '1' || status?.toString().toLowerCase() === 'confirmed';
  }

  makePayment(booking: Booking) {
    console.log('Making payment for booking:', booking);
    
    if (booking.id) {
      // Use reservation ID if available
      this.router.navigate(['/payment', booking.id]);
    } else {
      // Fallback: Get reservation ID using PNR
      this.bookingService.getBookingByPNR(booking.pnr).subscribe({
        next: (bookingDetails: any) => {
          if (bookingDetails.id) {
            this.router.navigate(['/payment', bookingDetails.id]);
          } else {
            this.showToast('Unable to process payment. Please try again.', 'error');
          }
        },
        error: () => {
          this.showToast('Unable to process payment. Please try again.', 'error');
        }
      });
    }
  }

  private showToast(message: string, type: 'success' | 'error') {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      padding: 1rem 1.5rem;
      border-radius: 8px;
      color: white;
      font-weight: 500;
      z-index: 10000;
      animation: slideIn 0.3s ease-out;
      ${type === 'success' ? 'background: linear-gradient(135deg, #10b981, #059669);' : 'background: linear-gradient(135deg, #ef4444, #dc2626);'}
    `;

    document.body.appendChild(toast);
    setTimeout(() => {
      toast.remove();
    }, 3000);
  }
}
