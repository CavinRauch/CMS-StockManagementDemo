import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    // Material
    MatCardModule, MatFormFieldModule, MatInputModule, MatCheckboxModule,
    MatButtonModule, MatIconModule, MatSnackBarModule, MatProgressSpinnerModule
  ],
  template: `
  <div class="page">
    <mat-card class="card">
      <h1>Sign in</h1>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <mat-form-field appearance="outline" class="full">
          <mat-label>Username or Email</mat-label>
          <input matInput formControlName="user" autocomplete="username"
                 (keyup.enter)="submit()">
          <mat-error *ngIf="form.controls.user.hasError('required')">
            Username or email is required
          </mat-error>
          <mat-error *ngIf="form.controls.user.hasError('email') && emailMode()">
            Enter a valid email
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full">
          <mat-label>Password</mat-label>
          <input matInput [type]="hide() ? 'password' : 'text'"
                 formControlName="pass" autocomplete="current-password"
                 (keyup.enter)="submit()">
          <button type="button" mat-icon-button matSuffix (click)="toggle()"
                  [attr.aria-label]="hide() ? 'Show password' : 'Hide password'"
                  [attr.aria-pressed]="!hide()">
            <mat-icon>{{ hide() ? 'visibility' : 'visibility_off' }}</mat-icon>
          </button>
          <mat-error *ngIf="form.controls.pass.hasError('required')">
            Password is required
          </mat-error>
          <mat-error *ngIf="form.controls.pass.hasError('minlength')">
            Minimum 8 characters
          </mat-error>
        </mat-form-field>

        <div class="row">
          <mat-checkbox formControlName="remember">Remember me</mat-checkbox>
          <a class="link" routerLink="/forgot" *ngIf="showForgot">Forgot password?</a>
        </div>

        <button mat-raised-button color="primary" class="full" [disabled]="form.invalid || busy()">
          <ng-container *ngIf="!busy(); else loading">
            Sign in
          </ng-container>
          <ng-template #loading>
            <span class="btn-flex">
              <mat-progress-spinner mode="indeterminate" diameter="18" strokeWidth="3"></mat-progress-spinner>
              Signing in…
            </span>
          </ng-template>
        </button>

        <div class="hint">
            Don’t have an account?
            <a routerLink="/register">Create one</a>
        </div>
      </form>
    </mat-card>
  </div>
  `,
  styles: [`
    .page { min-height: calc(100dvh - 64px); display: grid; place-items: center; padding: 24px; }
    .card { width: 100%; max-width: 420px; padding: 20px; }
    .full { width: 100%; }
    .row { display: flex; align-items: center; justify-content: space-between; margin: 6px 0 16px; }
    .link { text-decoration: none; font-size: .9rem; }
    .hint { margin-top: 12px; opacity: .75; }
    .btn-flex { display: inline-flex; align-items: center; gap: 8px; }
    h2 { margin: 4px 0 16px; font-weight: 600; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private snack = inject(MatSnackBar);
  private route = inject(ActivatedRoute);

  // UI state signals
  hide = signal(true);
  busy = signal(false);

  // feature flags
  showForgot = false;   // toggle if you add a forgot flow
  devHints = true;      // hide in prod if you want

  form = this.fb.group({
    user: ['', [Validators.required]],            // optionally add Validators.email when emailMode() is true
    pass: ['', [Validators.required, Validators.minLength(8)]],
    remember: [true]
  });

  ngOnInit() {
    const reason = this.route.snapshot.queryParamMap.get('reason');
    if (reason) {
      const msg =
        reason === 'expired'   ? 'Your session expired. Please sign in again.' :
        reason === 'forbidden' ? 'You don’t have permission to view that page. Sign in with a different account?' :
                                'Please sign in to continue.';
      this.snack.open(msg, 'OK', { duration: 3500 });
    }
  }

  emailMode() {
    const v = this.form.controls.user.value ?? '';
    return v.includes('@');
  }

  toggle() { this.hide.set(!this.hide()); }

  submit() {
    if (this.form.invalid || this.busy()) 
        return;
    
    const { user, pass, remember } = this.form.value;
    this.busy.set(true);

    this.auth.login(user!, pass!, !!remember)
    .pipe(finalize(() => this.busy.set(false)))
    .subscribe({
        next: () => {
            this.snack.open('Signed in', 'OK', { duration: 1500 });
            const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/stock';
            this.router.navigateByUrl(returnUrl);
        },
        error: (err) => {
            const msg = err?.error?.title || err?.error?.message || 'Invalid credentials';
            this.snack.open(msg, 'Dismiss', { duration: 3500 });
        }
    });
  }
}