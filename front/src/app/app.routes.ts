import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/components/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'categories',
        loadComponent: () => import('./features/categories/components/category-list/category-list.component').then(m => m.CategoryListComponent)
      },
      {
        path: 'categories/new',
        loadComponent: () => import('./features/categories/components/category-form/category-form.component').then(m => m.CategoryFormComponent)
      },
      {
        path: 'categories/edit/:id',
        loadComponent: () => import('./features/categories/components/category-form/category-form.component').then(m => m.CategoryFormComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./features/products/components/product-list/product-list.component').then(m => m.ProductListComponent)
      },
      {
        path: 'products/new',
        loadComponent: () => import('./features/products/components/product-form/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'products/edit/:id',
        loadComponent: () => import('./features/products/components/product-form/product-form.component').then(m => m.ProductFormComponent)
      },
      {
        path: 'tables',
        loadComponent: () => import('./features/tables/components/table-list/table-list.component').then(m => m.TableListComponent)
      },
      {
        path: 'tables/new',
        loadComponent: () => import('./features/tables/components/table-form/table-form.component').then(m => m.TableFormComponent)
      },
      {
        path: 'tables/edit/:id',
        loadComponent: () => import('./features/tables/components/table-form/table-form.component').then(m => m.TableFormComponent)
      },
      {
        path: 'users',
        canActivate: [RoleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/users/components/user-list/user-list.component').then(m => m.UserListComponent)
      },
      {
        path: 'users/new',
        canActivate: [RoleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/users/components/user-form/user-form.component').then(m => m.UserFormComponent)
      },
      {
        path: 'users/edit/:id',
        canActivate: [RoleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/users/components/user-form/user-form.component').then(m => m.UserFormComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
