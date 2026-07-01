import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div *ngIf="authService.isLoggedIn(); else loginTemplate" class="app-container">
      <nav class="sidebar">
        <div class="sidebar-header">
          <h2>🍺 BarSystem</h2>
        </div>
        <ul class="nav-menu">
          <li><a routerLink="/dashboard" routerLinkActive="active">📊 Dashboard</a></li>
          <li><a routerLink="/categories" routerLinkActive="active">📁 Categorias</a></li>
          <li><a routerLink="/products" routerLinkActive="active">🍔 Produtos</a></li>
          <li><a routerLink="/tables" routerLinkActive="active">🪑 Mesas</a></li>
          <li *ngIf="authService.hasRole('Admin')">
            <a routerLink="/users" routerLinkActive="active">👥 Usuários</a>
          </li>
        </ul>
        <div class="sidebar-footer">
          <div class="user-info">
            <p>{{ authService.getCurrentUser()?.name }}</p>
            <small>{{ authService.getCurrentUser()?.role }}</small>
          </div>
          <button (click)="logout()" class="btn-logout">Sair</button>
        </div>
      </nav>
      <main class="main-content">
        <router-outlet></router-outlet>
      </main>
    </div>
    <ng-template #loginTemplate>
      <router-outlet></router-outlet>
    </ng-template>
  `,
  styles: [`
    .app-container {
      display: flex;
      min-height: 100vh;
    }
    .sidebar {
      width: 250px;
      background: #2c3e50;
      color: white;
      display: flex;
      flex-direction: column;
    }
    .sidebar-header {
      padding: 20px;
      background: #34495e;
    }
    .sidebar-header h2 {
      margin: 0;
      font-size: 24px;
    }
    .nav-menu {
      list-style: none;
      padding: 0;
      margin: 0;
      flex: 1;
    }
    .nav-menu li a {
      display: block;
      padding: 15px 20px;
      color: white;
      text-decoration: none;
      transition: background 0.3s;
    }
    .nav-menu li a:hover,
    .nav-menu li a.active {
      background: #34495e;
    }
    .sidebar-footer {
      padding: 20px;
      background: #34495e;
    }
    .user-info {
      margin-bottom: 10px;
    }
    .user-info p {
      margin: 0;
      font-weight: bold;
    }
    .user-info small {
      color: #95a5a6;
    }
    .btn-logout {
      width: 100%;
      padding: 10px;
      background: #e74c3c;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-weight: bold;
    }
    .btn-logout:hover {
      background: #c0392b;
    }
    .main-content {
      flex: 1;
      padding: 20px;
      background: #ecf0f1;
    }
  `]
})
export class AppComponent {
  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
