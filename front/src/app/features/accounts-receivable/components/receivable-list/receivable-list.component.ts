import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-receivable-list.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './receivable-list.component.html',
  styleUrls: ['./receivable-list.component.css']
})
export class ReceivableListComponent {}
