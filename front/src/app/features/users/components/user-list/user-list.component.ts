import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { User } from '../../../../shared/models/user.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-header">
      <h1>Usuários</h1>
      <a routerLink="/users/new" class="btn btn-success">+ Novo Usuário</a>
    </div>
    
    <div class="card">
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Email</th>
            <th>Role</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let user of users">
            <td>{{ user.name }}</td>
            <td>{{ user.email }}</td>
            <td><span class="badge">{{ user.role }}</span></td>
            <td>
              <span class="badge" [class.active]="user.isActive" [class.inactive]="!user.isActive">
                {{ user.isActive ? 'Ativo' : 'Inativo' }}
              </span>
            </td>
            <td>
              <a [routerLink]="['/users/edit', user.id]" class="btn btn-primary btn-sm">Editar</a>
              <button (click)="delete(user.id)" class="btn btn-danger btn-sm">Excluir</button>
            </td>
          </tr>
          <tr *ngIf="users.length === 0">
            <td colspan="5" class="text-center">Nenhum usuário cadastrado</td>
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
      background: #3498db;
      color: white;
    }
    .badge.active {
      background: #27ae60;
    }
    .badge.inactive {
      background: #95a5a6;
    }
    .btn-sm {
      padding: 5px 10px;
      font-size: 12px;
      margin-right: 5px;
    }
    .text-center {
      text-align: center;
      color: #7f8c8d;
    }
  `]
})
export class UserListComponent implements OnInit {
  users: User[] = [];

  constructor(
    private http: HttpClient,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.http.get<User[]>('http://localhost:5020/api/users').subscribe({
      next: (data) => this.users = data,
      error: (err) => this.toastr.error('Erro ao carregar usuários')
    });
  }

  delete(id: string): void {
    if (confirm('Tem certeza que deseja excluir este usuário?')) {
      this.http.delete(`http://localhost:5020/api/users/${id}`).subscribe({
        next: () => {
          this.toastr.success('Usuário excluído com sucesso');
          this.loadUsers();
        },
        error: (err) => this.toastr.error('Erro ao excluir usuário')
      });
    }
  }
}
