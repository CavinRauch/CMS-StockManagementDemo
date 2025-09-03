import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { getToken, isExpired } from './jwt.util';

export const authGuard: CanActivateFn = (route, state): boolean | UrlTree => {
  const router = inject(Router);
  const token = getToken();

  if (!token) {
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url, reason: 'auth' }   // not logged in
    });
  }
  if (isExpired(token)) {
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url, reason: 'expired' } // token expired
    });
  }
  return true;
};