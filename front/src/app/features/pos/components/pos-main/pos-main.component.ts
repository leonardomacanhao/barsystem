import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pos-main.component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pos-main.component.html',
  styleUrls: ['./pos-main.component.css']
})
export class PosMainComponent {}
