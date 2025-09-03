import { Routes } from '@angular/router';
import { StockListComponent } from './stock/list/stock-list.component';
import { StockDetailComponent } from './stock/detail/stock-detail.component';
import { LoginComponent } from './auth/login.component';
import { authGuard } from './core/auth.guard';
import { roleGuard } from './core/role.guard';
import { ForbiddenComponent } from './core/forbidden.component';
import { RegisterComponent } from './auth/register.component';

export const routes: Routes = [
  { path: '', component: StockListComponent, pathMatch: 'full' },

  { path: 'stock', component: StockListComponent, canActivate: [authGuard] },  
  { path: 'stock/new', component: StockDetailComponent, canActivate: [authGuard, roleGuard(['Admin','Sales'])] },
  { path: 'stock/:id', component: StockDetailComponent, canActivate: [authGuard, roleGuard(['Admin','Sales'])] },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forbidden', component: ForbiddenComponent },

  { path: '**', redirectTo: '' }
];