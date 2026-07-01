import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h1>{{ isEdit ? 'Editar' : 'Novo' }} Usuário</h1>
      <a routerLink="/users" class="btn btn-primary">Voltar</a>
    </div>
    
    <div class="card">
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label>Nome *</label>
          <input type="text" formControlName="name" placeholder="Nome completo">
        </div>
        <div class="form-group">
          <label>Email *</label>
          <input type="email" formControlName="email" placeholder="email@exemplo.com">
        </div>
        <div class="form-group" *ngIf="!isEdit">
          <label>Senha *</label>
          <input type="password" formControlName="password" placeholder="Mínimo 6 caracteres">
        </div>
        <div class="form-group">
          <label>Role *</label>
          <select formControlName="role">
            <option value="">Selecione</option>
            <option value="Admin">Administrador</option>
            <option value="Manager">Gerente</option>
            <option value="Waiter">Garçom</option>
            <option value="Cashier">Caixa</option>
          </select>
        </div>
        <button type="submit" class="btn btn-success" [disabled]="loading || form.invalid">
          {{ loading ? 'Salvando...' : 'Salvar' }}
        </button>
      </form>
    </div>
  `
})
export class UserFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  loading = false;
  userId?: string;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      role: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.params['id'];
    if (this.userId) {
      this.isEdit = true;
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
      this.loadUser();
    } else {
      this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
      this.form.get('password')?.updateValueAndValidity();
    }
  }

  loadUser(): void {
    if (!this.userId) return;
    
    this.http.get<any>(`http://localhost:5020/api/users/${this.userId}`).subscribe({
      next: (user) => {
        this.form.patchValue({
          name: user.name,
          email: user.email,
          role: user.role
        });
      },
      error: (err) => this.toastr.error('Erro ao carregar usuário')
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const request = this.form.value;

    const url = this.isEdit 
      ? `http://localhost:5020/api/users/${this.userId}`
      : 'http://localhost:5020/api/users';

    const method = this.isEdit ? 'put' : 'post';

    this.http.request(method, url, { body: request }).subscribe({
      next: () => {
        this.toastr.success(`Usuário ${this.isEdit ? 'atualizado' : 'criado'} com sucesso`);
        this.router.navigate(['/users']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Erro ao salvar usuário');
        this.loading = false;
      }
    });
  }
}
