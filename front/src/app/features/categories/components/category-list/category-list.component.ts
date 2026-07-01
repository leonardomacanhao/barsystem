import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../shared/models/category.model';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterLink,
    PageHeaderComponent,
    CardComponent,
    EmptyStateComponent
  ],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css']
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];

  constructor(
    private categoryService: CategoryService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data,
      error: () => this.toastr.error('Erro ao carregar categorias')
    });
  }

  delete(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta categoria?')) {
      this.categoryService.delete(id).subscribe({
        next: () => {
          this.toastr.success('Categoria excluída com sucesso');
          this.loadCategories();
        },
        error: () => this.toastr.error('Erro ao excluir categoria')
      });
    }
  }
}
