import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { TrainSearchComponent } from './components/train-search/train-search';
import { DashboardComponent } from './components/dashboard/dashboard';
import { BookingComponent } from './components/booking/booking';
import { PaymentComponent } from './components/payment/payment';
import { MyBookingsComponent } from './components/my-bookings/my-bookings';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard';
import { AdminTrainsComponent } from './components/admin-trains/admin-trains';
import { AdminBookingsComponent } from './components/admin-bookings/admin-bookings';
import { authGuard } from './guards/auth-guard';
import { userGuard } from './guards/user-guard';
import { adminGuard } from './guards/admin-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/train-search', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'train-search', component: TrainSearchComponent },
  { path: 'booking/:trainId', component: BookingComponent, canActivate: [userGuard] },
  { path: 'payment/:reservationId', component: PaymentComponent, canActivate: [userGuard] },
  { path: 'payment', component: PaymentComponent, canActivate: [userGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [userGuard] },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [userGuard] },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [adminGuard] },
  { path: 'admin/trains', component: AdminTrainsComponent, canActivate: [adminGuard] },
  { path: 'admin/bookings', component: AdminBookingsComponent, canActivate: [adminGuard] },
  { path: '**', redirectTo: '/train-search' }
];
