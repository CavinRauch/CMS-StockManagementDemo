import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

function matchPasswords(group: AbstractControl): ValidationErrors | null {
  const p = group.get('password')?.value;
  const c = group.get('confirm')?.value;
  return p && c && p !== c ? { mismatch: true } : null;
}

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatSnackBarModule, MatProgressSpinnerModule
  ],
  template: `
  <div class="page">
    <mat-card class="card">
      <h2>Create account</h2>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <mat-form-field appearance="outline" class="full">
          <mat-label>Username</mat-label>
          <input matInput formControlName="userName" autocomplete="username">
          <mat-error *ngIf="form.controls.userName.hasError('required')">Username is required</mat-error>
          <mat-error *ngIf="form.controls.userName.hasError('minlength')">Min 3 characters</mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" autocomplete="email">
          <mat-error *ngIf="form.controls.email.hasError('required')">Email is required</mat-error>
          <mat-error *ngIf="form.controls.email.hasError('email')">Enter a valid email</mat-error>
        </mat-form-field>

        <div formGroupName="pwd">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Password</mat-label>
            <input matInput [type]="hide() ? 'password' : 'text'" formControlName="password" autocomplete="new-password">
            <button type="button" mat-icon-button matSuffix (click)="toggle()">
              <mat-icon>{{ hide() ? 'visibility' : 'visibility_off' }}</mat-icon>
            </button>
            <mat-error *ngIf="pwd.controls.password.hasError('required')">Password is required</mat-error>
            <mat-error *ngIf="pwd.controls.password.hasError('minlength')">Min 8 characters</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="full">
            <mat-label>Confirm password</mat-label>
            <input matInput [type]="hide() ? 'password' : 'text'" formControlName="confirm" autocomplete="new-password">
            <mat-error *ngIf="pwd.hasError('mismatch')">Passwords do not match</mat-error>
          </mat-form-field>
        </div>

        <button mat-raised-button color="primary" class="full" [disabled]="form.invalid || busy()">
          <ng-container *ngIf="!busy(); else loading">Register</ng-container>
          <ng-template #loading>
            <span class="btn-flex">
              <mat-progress-spinner mode="indeterminate" diameter="18" strokeWidth="3"></mat-progress-spinner>
              Creating account…
            </span>
          </ng-template>
        </button>

        <div class="hint">
          Already have an account?
          <a routerLink="/login">Sign in</a>
        </div>
      </form>
    </mat-card>
  </div>
  `,
  styles: [`
    .page { min-height: calc(100dvh - 64px); display: grid; place-items: center; padding: 24px; }
    .card { width: 100%; max-width: 480px; padding: 20px; }
    .full { width: 100%; }
    .btn-flex { display: inline-flex; align-items: center; gap: 8px; }
    h2 { margin: 4px 0 16px; font-weight: 600; }
    .hint { margin-top: 12px; }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private snack = inject(MatSnackBar);

  hide = signal(true);
  busy = signal(false);

  form = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    pwd: this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirm: ['', [Validators.required]]
    }, { validators: matchPasswords })
  });

  get pwd() { return this.form.get('pwd') as any; }

  toggle() { this.hide.set(!this.hide()); }

  submit() {
    if (this.form.invalid || this.busy()) return;
    const { userName, email } = this.form.value as { userName: string; email: string; };
    const password = this.pwd.get('password')!.value as string;

    this.busy.set(true);
    this.auth.register(userName, email, password).subscribe({
      next: () => {
        this.snack.open('Account created. Please sign in.', 'OK', { duration: 2500 });
        this.router.navigate(['/login'], { queryParams: { user: userName } });
      },
      error: (err) => {
        const msg = err?.error?.title || err?.error?.message || 'Registration failed';
        this.snack.open(msg, 'Dismiss', { duration: 4000 });
        this.busy.set(false);
      },
      complete: () => this.busy.set(false)
    });
  }
}
