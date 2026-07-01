import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TableService } from '../../../../core/services/table.service';
import { Table } from '../../../../shared/models/table.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-table-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-header">
      <h1>Mesas</h1>
      <a routerLink="/tables/new" class="btn btn-success">+ Nova Mesa</a>
    </div>
    
    <div class="card">
      <table>
        <thead>
          <tr>
            <th>Número</th>
            <th>Capacidade</th>
            <th>Localização</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let table of tables">
            <td><strong>Mesa {{ table.number }}</strong></td>
            <td>{{ table.capacity }} pessoas</td>
            <td>{{ table.location || '-' }}</td>
            <td>
              <span class="badge" [class.free]="table.status === 'Free'" [class.occupied]="table.status === 'Occupied'" [class.reserved]="table.status === 'Reserved'">
                {{ getStatusLabel(table.status) }}
              </span>
            </td>
            <td>
              <a [routerLink]="['/tables/edit', table.id]" class="btn btn-primary btn-sm">Editar</a>
              <button (click)="changeStatus(table)" class="btn btn-warning btn-sm">Alterar Status</button>
              <button (click)="delete(table.id)" class="btn btn-danger btn-sm">Excluir</button>
            </td>
          </tr>
          <tr *ngIf="tables.length === 0">
            <td colspan="5" class="text-center">Nenhuma mesa cadastrada</td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .badge {
      padding: 5px 10px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: bold;
    }
    .badge.free {
      background: #27ae60;
      color: white;
    }
    .badge.occupied {
      background: #e74c3c;
      color: white;
    }
    .badge.reserved {
      background: #f39c12;
      color: white;
    }
    .btn-sm {
      padding: 5px 10px;
      font-size: 12px;
      margin-right: 5px;
    }
    .btn-warning {
      background: #f39c12;
      color: white;
    }
    .text-center {
      text-align: center;
      color: #7f8c8d;
    }
  `]
})
export class TableListComponent implements OnInit {
  tables: Table[] = [];

  constructor(
    private tableService: TableService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadTables();
  }

  loadTables(): void {
    this.tableService.getAll().subscribe({
      next: (data) => this.tables = data,
      error: (err) => this.toastr.error('Erro ao carregar mesas')
    });
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Free': 'Livre',
      'Occupied': 'Ocupada',
      'Reserved': 'Reservada'
    };
    return labels[status] || status;
  }

  changeStatus(table: Table): void {
    const newStatus = prompt('Novo status (Free, Occupied, Reserved):', table.status);
    if (newStatus && ['Free', 'Occupied', 'Reserved'].includes(newStatus)) {
      this.tableService.updateStatus(table.id, { status: newStatus as any }).subscribe({
        next: () => {
          this.toastr.success('Status atualizado');
          this.loadTables();
        },
        error: (err) => this.toastr.error('Erro ao atualizar status')
      });
    }
  }

  delete(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta mesa?')) {
      this.tableService.delete(id).subscribe({
        next: () => {
          this.toastr.success('Mesa excluída com sucesso');
          this.loadTables();
        },
        error: (err) => this.toastr.error('Erro ao excluir mesa')
      });
    }
  }
}
