import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../shared/models/category.model';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { environment } from '../../../../environments/environment';
import { CardComponent } from '../../../../shared/components/card/card.component';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, PageHeaderComponent, CardComponent],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
  form: FormGroup;
  categories: Category[] = [];
  isEdit = false;
  loading = false;
  uploading = false;
  productId?: string;
  selectedFile?: File;
  imagePreview?: string;
  private readonly baseUrl = environment.apiUrl || '';

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: [null, [Validators.required, Validators.min(0.01)]],
      categoryId: ['', Validators.required],
      imageUrl: [''],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.productId = this.route.snapshot.params['id'];
    if (this.productId) {
      this.isEdit = true;
      this.loadProduct();
    }
  }

  getImageUrl(): string {
    if (this.imagePreview) {
      return this.imagePreview;
    }
    const imageUrl = this.form.get('imageUrl')?.value;
    if (imageUrl) {
      return imageUrl.startsWith('http') ? imageUrl : `${this.baseUrl}${imageUrl}`;
    }
    return '';
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data,
      error: () => this.toastr.error('Erro ao carregar categorias')
    });
  }

  loadProduct(): void {
    if (!this.productId) return;
    
    this.productService.getById(this.productId).subscribe({
      next: (product) => {
        this.form.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          categoryId: product.categoryId,
          imageUrl: product.imageUrl,
          isActive: product.isActive
        });
      },
      error: () => this.toastr.error('Erro ao carregar produto')
    });
  }

  onPriceInput(event: any): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    
    // Remove tudo exceto números, ponto e vírgula
    value = value.replace(/[^\d.,]/g, '');
    
    // Converte vírgula para ponto
    value = value.replace(',', '.');
    
    // Remove pontos duplicados
    const parts = value.split('.');
    if (parts.length > 2) {
      value = parts[0] + '.' + parts.slice(1).join('');
    }
    
    // Remove zeros à esquerda
    if (value.startsWith('0') && value.length > 1 && !value.startsWith('0.')) {
      value = value.replace(/^0+/, '') || '0';
    }
    
    // Limita a 2 casas decimais
    if (value.includes('.')) {
      const [integer, decimal] = value.split('.');
      if (decimal && decimal.length > 2) {
        value = integer + '.' + decimal.substring(0, 2);
      }
    }
    
    // Atualiza o input
    input.value = value;
    
    // Atualiza o form
    const numericValue = parseFloat(value) || null;
    this.form.patchValue({ price: numericValue }, { emitEvent: false });
  }

  onPriceBlur(): void {
    const value = this.form.get('price')?.value;
    if (value && value > 0) {
      // Formata com 2 casas decimais
      const formatted = parseFloat(value.toFixed(2));
      this.form.patchValue({ price: formatted }, { emitEvent: false });
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 2 * 1024 * 1024) {
        this.toastr.error('A imagem deve ter no máximo 2MB');
        return;
      }
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => this.imagePreview = e.target.result;
      reader.readAsDataURL(file);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) return;

    this.loading = true;
    let imageUrl = this.form.get('imageUrl')?.value;

    if (this.selectedFile) {
      this.uploading = true;
      try {
        const response = await this.productService.uploadImage(this.selectedFile).toPromise();
        imageUrl = response?.url;
        this.form.patchValue({ imageUrl });
      } catch (err) {
        this.toastr.error('Erro ao enviar imagem');
        this.loading = false;
        this.uploading = false;
        return;
      }
      this.uploading = false;
    }

    const request = { ...this.form.value, imageUrl };

    const operation = this.isEdit
      ? this.productService.update(this.productId!, request)
      : this.productService.create(request);

    operation.subscribe({
      next: () => {
        this.toastr.success(`Produto ${this.isEdit ? 'atualizado' : 'criado'} com sucesso`);
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Erro ao salvar produto');
        this.loading = false;
      }
    });
  }
}
