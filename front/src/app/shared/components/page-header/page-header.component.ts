import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-header">
      <div class="header-content">
        <div class="header-text">
          <h1>{{ title }}</h1>
          <p *ngIf="subtitle">{{ subtitle }}</p>
        </div>
        <div class="header-actions">
          <ng-content></ng-content>
          <a *ngIf="actionUrl && actionLabel" [routerLink]="actionUrl" class="btn-primary">
            <svg *ngIf="actionType === 'back'" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="19" y1="12" x2="5" y2="12"/>
              <polyline points="12 19 5 12 12 5"/>
            </svg>
            <svg *ngIf="actionType !== 'back'" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="5" x2="12" y2="19"/>
              <line x1="5" y1="12" x2="19" y2="12"/>
            </svg>
            {{ actionLabel }}
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      margin-bottom: 32px;
    }

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 16px;
    }

    .header-text h1 {
      font-size: 32px;
      font-weight: 700;
      color: #1d1d1f;
      margin: 0 0 6px 0;
      letter-spacing: -0.02em;
    }

    .header-text p {
      font-size: 15px;
      color: #86868b;
      margin: 0;
    }

    .header-actions {
      display: flex;
      gap: 12px;
      align-items: center;
    }

    .btn-primary {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      padding: 10px 20px;
      background: linear-gradient(135deg, #0071e3 0%, #0051a8 100%);
      color: white;
      border: none;
      border-radius: 10px;
      font-size: 14px;
      font-weight: 600;
      text-decoration: none;
      cursor: pointer;
      transition: all 0.2s ease;
      box-shadow: 0 2px 8px rgba(0, 113, 227, 0.25);
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 113, 227, 0.35);
    }

    @media (max-width: 768px) {
      .header-content {
        flex-direction: column;
        align-items: flex-start;
      }

      .header-actions {
        width: 100%;
      }

      .btn-primary {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class PageHeaderComponent {
  @Input() title: string = '';
  @Input() subtitle?: string;
  @Input() actionUrl?: string;
  @Input() actionLabel?: string;
  @Input() actionType: 'primary' | 'back' = 'primary';
}
