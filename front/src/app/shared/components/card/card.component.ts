import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card" [class.padded]="padded" [class.hover]="hover">
      <div class="card-header" *ngIf="title || subtitle">
        <div class="card-title-section">
          <h2 *ngIf="title" class="card-title">{{ title }}</h2>
          <p *ngIf="subtitle" class="card-subtitle">{{ subtitle }}</p>
        </div>
        <div class="card-header-actions">
          <ng-content select="[card-header]"></ng-content>
        </div>
      </div>
      <div class="card-body">
        <ng-content></ng-content>
      </div>
      <div class="card-footer" *ngIf="hasFooter">
        <ng-content select="[card-footer]"></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .card {
      background: white;
      border-radius: 16px;
      border: 1px solid rgba(0, 0, 0, 0.04);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
      overflow: hidden;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .card.hover:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      padding: 24px 28px;
      border-bottom: 1px solid rgba(0, 0, 0, 0.06);
    }

    .card.padded .card-header {
      padding: 28px;
    }

    .card-title-section {
      flex: 1;
    }

    .card-title {
      font-size: 20px;
      font-weight: 700;
      color: #1d1d1f;
      margin: 0 0 4px 0;
      letter-spacing: -0.01em;
    }

    .card-subtitle {
      font-size: 14px;
      color: #86868b;
      margin: 0;
    }

    .card-header-actions {
      display: flex;
      gap: 8px;
      margin-left: 16px;
    }

    .card-body {
      padding: 24px 28px;
    }

    .card.padded .card-body {
      padding: 28px;
    }

    .card-body:empty {
      padding: 0;
    }

    .card-footer {
      padding: 20px 28px;
      border-top: 1px solid rgba(0, 0, 0, 0.06);
      background: #fafafa;
    }

    @media (max-width: 768px) {
      .card-header,
      .card-body,
      .card-footer {
        padding-left: 20px;
        padding-right: 20px;
      }

      .card-header {
        flex-direction: column;
        gap: 12px;
        align-items: stretch;
      }

      .card-header-actions {
        margin-left: 0;
      }
    }
  `]
})
export class CardComponent {
  @Input() title?: string;
  @Input() subtitle?: string;
  @Input() padded: boolean = false;
  @Input() hover: boolean = false;
  @Input() hasFooter: boolean = false;
}
