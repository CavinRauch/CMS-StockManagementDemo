import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private tokenKey = 'jwt';

  private setToken(token: string, remember: boolean) {
    // clear both first
    localStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.tokenKey);
    // persist based on remember flag
    (remember ? localStorage : sessionStorage).setItem(this.tokenKey, token);
  }

  login(userNameOrEmail: string, password: string, remember = true) {
    return this.http
      .post<{ accessToken: string }>(`${environment.api}/auth/login`, { userName: userNameOrEmail, password })
      .pipe(tap(res => this.setToken(res.accessToken, remember)));
  }

  register(userName: string, email: string, password: string) {
    return this.http.post(`${environment.api}/auth/register`, { userName, email, password });
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.tokenKey);
  }

  get token(): string | null {
    return sessionStorage.getItem(this.tokenKey) ?? localStorage.getItem(this.tokenKey);
  }

  get isAuthenticated(): boolean { 
    return !!this.token; 
  }
}
