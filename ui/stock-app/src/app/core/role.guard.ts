import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { getToken, getRolesFromToken, isExpired } from './jwt.util';

export const roleGuard = (required: string[]): CanActivateFn =>
  (route, state): boolean | UrlTree => {
    const router = inject(Router);
    const token = getToken();
    if (!token) {
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url, reason: 'auth' } });
    }
    if (isExpired(token)) {
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url, reason: 'expired' } });
    }
    const ok = getRolesFromToken(token).some(r => required.map(x => x.toLowerCase()).includes(r.toLowerCase()));
    return ok
      ? true
      : router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url, reason: 'forbidden' } });
  };