import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const userGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated() && !authService.isAdmin()) {
    return true;
  }

  if (authService.isAdmin()) {
    router.navigate(['/admin']);
  } else {
    router.navigate(['/login']);
  }
  return false;
};