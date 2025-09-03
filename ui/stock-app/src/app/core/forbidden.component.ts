import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  selector: 'app-forbidden',
  imports: [CommonModule, RouterLink, MatButtonModule],
  template: `
    <div class="wrap">
      <h2>403 – Forbidden</h2>
      <p>You don't have permission to access this page.</p>
      <a mat-raised-button color="primary" routerLink="/">Go Home</a>
    </div>
  `,
  styles: [`.wrap{min-height:60vh;display:grid;place-items:center;gap:12px;text-align:center}`]
})
export class ForbiddenComponent {}
