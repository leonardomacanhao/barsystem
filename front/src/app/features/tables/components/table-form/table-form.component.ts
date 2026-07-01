import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TableService } from '../../../../core/services/table.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-table-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h1>{{ isEdit ? 'Editar' : 'Nova' }} Mesa</h1>
      <a routerLink="/tables" class="btn btn-primary">Voltar</a>
    </div>
    
    <div class="card">
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label>Número da Mesa *</label>
          <input type="number" formControlName="number" placeholder="Ex: 1">
        </div>
        <div class="form-group">
          <label>Capacidade (pessoas) *</label>
          <input type="number" formControlName="capacity" placeholder="Ex: 4">
        </div>
        <div class="form-group">
          <label>Localização</label>
          <input type="text" formControlName="location" placeholder="Ex: Salão Principal, Varanda">
        </div>
        <button type="submit" class="btn btn-success" [disabled]="loading || form.invalid">
          {{ loading ? 'Salvando...' : 'Salvar' }}
        </button>
      </form>
    </div>
  `
})
export class TableFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  loading = false;
  tableId?: string;

  constructor(
    private fb: FormBuilder,
    private tableService: TableService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      number: [0, [Validators.required, Validators.min(1)]],
      capacity: [0, [Validators.required, Validators.min(1), Validators.max(100)]],
      location: ['']
    });
  }

  ngOnInit(): void {
    this.tableId = this.route.snapshot.params['id'];
    if (this.tableId) {
      this.isEdit = true;
      this.loadTable();
    }
  }

  loadTable(): void {
    if (!this.tableId) return;
    
    this.tableService.getById(this.tableId).subscribe({
      next: (table) => {
        this.form.patchValue({
          number: table.number,
          capacity: table.capacity,
          location: table.location
        });
      },
      error: (err) => this.toastr.error('Erro ao carregar mesa')
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const request = this.form.value;

    const operation = this.isEdit
      ? this.tableService.update(this.tableId!, request)
      : this.tableService.create(request);

    operation.subscribe({
      next: () => {
        this.toastr.success(`Mesa ${this.isEdit ? 'atualizada' : 'criada'} com sucesso`);
        this.router.navigate(['/tables']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Erro ao salvar mesa');
        this.loading = false;
      }
    });
  }
}
