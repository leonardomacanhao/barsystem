import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../shared/models/product.model';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { environment } from '../../../../environments/environment';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    RouterLink,
    PageHeaderComponent,
    CardComponent,
    EmptyStateComponent
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  searchTerm: string = '';
  private readonly baseUrl = environment.apiUrl || '';

  constructor(
    private productService: ProductService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  getImageUrl(imageUrl: string): string {
    return imageUrl.startsWith('http') ? imageUrl : `${this.baseUrl}${imageUrl}`;
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.filteredProducts = data;
      },
      error: () => this.toastr.error('Erro ao carregar produtos')
    });
  }

  filterProducts(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredProducts = this.products;
      return;
    }

    this.filteredProducts = this.products.filter(product => 
      product.name.toLowerCase().includes(term) ||
      (product.description && product.description.toLowerCase().includes(term)) ||
      (product.categoryName && product.categoryName.toLowerCase().includes(term))
    );
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.filteredProducts = this.products;
  }

  delete(id: string): void {
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.productService.delete(id).subscribe({
        next: () => {
          this.toastr.success('Produto excluído com sucesso');
          this.loadProducts();
        },
        error: () => this.toastr.error('Erro ao excluir produto')
      });
    }
  }
}
