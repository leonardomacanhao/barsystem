import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-delivery-integration.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './delivery-integration.component.html',
  styleUrls: ['./delivery-integration.component.css']
})
export class DeliveryIntegrationComponent {}
