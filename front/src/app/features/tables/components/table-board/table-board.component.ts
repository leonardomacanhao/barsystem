import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-table-board',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './table-board.component.html',
  styleUrls: ['./table-board.component.css']
})
export class TableBoardComponent {}
