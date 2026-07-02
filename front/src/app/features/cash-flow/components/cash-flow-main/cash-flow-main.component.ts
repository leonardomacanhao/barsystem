import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cash-flow-main.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cash-flow-main.component.html',
  styleUrls: ['./cash-flow-main.component.css']
})
export class CashFlowMainComponent {}
