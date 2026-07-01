export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  categoryName?: string;
  imageUrl?: string;
  isActive: boolean;
  tenantId: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  imageUrl?: string;
  isActive?: boolean;
}

export interface UpdateProductRequest {
  name?: string;
  description?: string;
  price?: number;
  categoryId?: string;
  imageUrl?: string;
  isActive?: boolean;
}
