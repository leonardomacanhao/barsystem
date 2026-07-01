import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { LoginRequest, LoginResponse, RegisterRequest, User } from '../../shared/models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl ? `${environment.apiUrl}/api/auth` : '/api/auth';
  private readonly tokenKey = 'auth_token';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private isBrowser: boolean;
  
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    if (this.isBrowser) {
      this.loadStoredUser();
    }
  }

  private loadStoredUser(): void {
    if (!this.isBrowser) return;
    
    const token = localStorage.getItem(this.tokenKey);
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        const user: User = {
          id: decoded.UserId || decoded.sub,
          name: decoded.Name || decoded.name,
          email: decoded.Email || decoded.email,
          role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role,
          groupId: decoded.GroupId,
          groupName: decoded.GroupName,
          tenantId: decoded.TenantId !== '00000000-0000-0000-0000-000000000000' ? decoded.TenantId : undefined,
          tenantName: decoded.TenantName !== 'Todos os bares' ? decoded.TenantName : undefined,
          isActive: true,
          createdAt: new Date().toISOString()
        };
        this.currentUserSubject.next(user);
      } catch (error) {
        this.logout();
      }
    }
  }

  private mapResponseToUser(response: LoginResponse): User {
    return {
      id: response.userId,
      name: response.name,
      email: response.email,
      role: response.role,
      groupId: response.groupId,
      groupName: response.groupName,
      tenantId: response.tenantId,
      tenantName: response.tenantName,
      isActive: true,
      createdAt: new Date().toISOString()
    };
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (this.isBrowser) {
          localStorage.setItem(this.tokenKey, response.token);
        }
        this.currentUserSubject.next(this.mapResponseToUser(response));
      })
    );
  }

  register(request: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/register`, request).pipe(
      tap(response => {
        if (this.isBrowser) {
          localStorage.setItem(this.tokenKey, response.token);
        }
        this.currentUserSubject.next(this.mapResponseToUser(response));
      })
    );
  }

  switchTenant(tenantId: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/switch-tenant`, { tenantId }).pipe(
      tap(response => {
        if (this.isBrowser) {
          localStorage.setItem(this.tokenKey, response.token);
        }
        this.currentUserSubject.next(this.mapResponseToUser(response));
      })
    );
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.tokenKey);
    }
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser) return false;
    return !!localStorage.getItem(this.tokenKey);
  }

  getToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem(this.tokenKey);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === role;
  }

  isGroupAdmin(): boolean {
    return this.hasRole('GroupAdmin');
  }

  isBarManager(): boolean {
    return this.hasRole('BarManager');
  }

  hasTenant(): boolean {
    const user = this.currentUserSubject.value;
    return !!user?.tenantId;
  }
}
