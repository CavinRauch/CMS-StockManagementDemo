import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from './core/auth.service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterLink, RouterOutlet, MatToolbarModule, MatButtonModule],
  template: `
    <mat-toolbar color="primary">
      <span>Stock Management Demo</span>
      <span class="spacer"></span>
      <a mat-button routerLink="/stock">Stock</a>
      @if(auth.isAuthenticated){
        <a mat-button (click)="logout()">Logout</a>
      }@else{
        <a mat-button routerLink="/login">Login</a>
      }
    </mat-toolbar>

    <div class="page">
      <router-outlet></router-outlet>
    </div>`,
  styles: [`
    .spacer { flex: 1; }
    .page { padding: 16px; }
  `]
})

export class AppComponent {
  private router = inject(Router);
  auth = inject(AuthService);
  
  title = 'stock-app';

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
