import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-kitchen-display.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './kitchen-display.component.html',
  styleUrls: ['./kitchen-display.component.css']
})
export class KitchenDisplayComponent {}
