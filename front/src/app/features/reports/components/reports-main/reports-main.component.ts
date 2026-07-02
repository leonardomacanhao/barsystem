import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-reports-main.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './reports-main.component.html',
  styleUrls: ['./reports-main.component.css']
})
export class ReportsMainComponent {}
