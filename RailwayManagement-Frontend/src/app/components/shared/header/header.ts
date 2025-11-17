import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [RouterModule, CommonModule],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class HeaderComponent implements OnInit, OnDestroy {
  hasPendingPayment = false;
  pendingPaymentData: any = null;
  private routerSubscription?: Subscription;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getUserName(): string {
    return this.authService.currentUserValue?.name || 'User';
  }

  getUserRole(): string {
    return this.authService.currentUserValue?.role || 'User';
  }

  getProfileImage(): string {
    const name = this.getUserName();
    const initials = name.split(' ').map(n => n[0]).join('').toUpperCase();
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(name)}&background=667eea&color=fff&size=40&font-size=0.6&rounded=true&format=svg`;
  }

  ngOnInit() {
    this.checkPendingPayment();
    
    // Check for pending payment on route changes
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkPendingPayment();
      });
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  checkPendingPayment() {
    const pending = sessionStorage.getItem('pendingPayment');
    if (pending) {
      this.pendingPaymentData = JSON.parse(pending);
      this.hasPendingPayment = true;
    } else {
      this.hasPendingPayment = false;
      this.pendingPaymentData = null;
    }
  }

  resumePayment() {
    if (this.pendingPaymentData) {
      this.router.navigate(['/payment'], { 
        queryParams: { 
          resumePayment: true,
          reservationId: this.pendingPaymentData.reservationId 
        } 
      });
    }
  }

  dismissPaymentAlert() {
    sessionStorage.removeItem('pendingPayment');
    this.hasPendingPayment = false;
    this.pendingPaymentData = null;
  }
}
