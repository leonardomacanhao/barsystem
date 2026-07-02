import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-payable-list.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payable-list.component.html',
  styleUrls: ['./payable-list.component.css']
})
export class PayableListComponent {}
