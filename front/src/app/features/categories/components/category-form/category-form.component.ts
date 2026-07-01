import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CategoryService } from '../../../../core/services/category.service';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { CardComponent } from '../../../../shared/components/card/card.component';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, PageHeaderComponent, CardComponent],
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.css']
})
export class CategoryFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  loading = false;
  categoryId?: string;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.categoryId = this.route.snapshot.params['id'];
    if (this.categoryId) {
      this.isEdit = true;
      this.loadCategory();
    }
  }

  loadCategory(): void {
    if (!this.categoryId) return;
    
    this.categoryService.getById(this.categoryId).subscribe({
      next: (category) => {
        this.form.patchValue({
          name: category.name,
          description: category.description
        });
      },
      error: () => this.toastr.error('Erro ao carregar categoria')
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const request = this.form.value;

    const operation = this.isEdit
      ? this.categoryService.update(this.categoryId!, request)
      : this.categoryService.create(request);

    operation.subscribe({
      next: () => {
        this.toastr.success(`Categoria ${this.isEdit ? 'atualizada' : 'criada'} com sucesso`);
        this.router.navigate(['/categories']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Erro ao salvar categoria');
        this.loading = false;
      }
    });
  }
}
