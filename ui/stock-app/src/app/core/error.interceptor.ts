import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export interface NormalizedError {
  status: number;
  message: string;
  validation?: Record<string, string[]>;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snack = inject(MatSnackBar);

  return next(req).pipe(
    catchError((e: HttpErrorResponse) => {
        const n: NormalizedError = {
            status: e.status ?? 0,
            message:
            e.error?.title ??
            e.error?.message ??
            (e.status === 0 ? 'Network error' : e.message) ??
            'Unexpected error'
        };

        // ASP.NET + FluentValidation returns { errors: { key: [msgs] } }
        const errors = e.error?.errors;
        if (errors && typeof errors === 'object') {
            n.validation = errors as Record<string, string[]>;
        }

        // Only toast non-validation errors
        if (!n.validation) {
            snack.open(n.message, 'Dismiss', { duration: 3500 });
        }

        if (e.status === 401 || e.status === 403) {
            const onLogin = router.url.startsWith('/login');
            if (!onLogin) {
                const reason = e.status === 401 ? 'expired' : 'forbidden';
                router.navigate(['/login'], { queryParams: { returnUrl: router.url, reason } });
            }
        }

      return throwError(() => n); // rethrow normalized error
    })
  );
};
