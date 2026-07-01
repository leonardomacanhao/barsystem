import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="empty-state">
      <div class="empty-icon">
        <ng-content select="[icon]"></ng-content>
      </div>
      <h3>{{ title }}</h3>
      <p>{{ description }}</p>
      <button *ngIf="actionLabel && actionUrl" [routerLink]="actionUrl" class="btn-primary">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="12" y1="5" x2="12" y2="19"/>
          <line x1="5" y1="12" x2="19" y2="12"/>
        </svg>
        {{ actionLabel }}
      </button>
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 64px 32px;
      text-align: center;
    }

    .empty-icon {
      width: 64px;
      height: 64px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #f5f5f7 0%, #e8e8ed 100%);
      border-radius: 16px;
      margin-bottom: 20px;
      color: #86868b;
    }

    .empty-icon svg {
      width: 32px;
      height: 32px;
    }

    .empty-state h3 {
      font-size: 18px;
      font-weight: 600;
      color: #1d1d1f;
      margin: 0 0 8px 0;
      letter-spacing: -0.01em;
    }

    .empty-state p {
      font-size: 15px;
      color: #86868b;
      margin: 0 0 24px 0;
      max-width: 400px;
      line-height: 1.5;
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
      .empty-state {
        padding: 48px 20px;
      }
    }
  `]
})
export class EmptyStateComponent {
  @Input() title: string = '';
  @Input() description: string = '';
  @Input() actionLabel?: string;
  @Input() actionUrl?: string;
}
