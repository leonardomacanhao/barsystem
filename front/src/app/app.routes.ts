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
        path: 'orders',
        loadComponent: () => import('./features/orders/components/order-list/order-list.component').then(m => m.OrderListComponent)
      },
      {
        path: 'pos',
        loadComponent: () => import('./features/pos/components/pos-main/pos-main.component').then(m => m.PosMainComponent)
      },
      {
        path: 'kitchen',
        loadComponent: () => import('./features/kitchen/components/kitchen-display/kitchen-display.component').then(m => m.KitchenDisplayComponent)
      },
      {
        path: 'accounts',
        loadComponent: () => import('./features/accounts/components/account-list/account-list.component').then(m => m.AccountListComponent)
      },
      {
        path: 'customers',
        loadComponent: () => import('./features/customers/components/customer-list/customer-list.component').then(m => m.CustomerListComponent)
      },
      {
        path: 'employees',
        loadComponent: () => import('./features/employees/components/employee-list/employee-list.component').then(m => m.EmployeeListComponent)
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./features/suppliers/components/supplier-list/supplier-list.component').then(m => m.SupplierListComponent)
      },
      {
        path: 'cash-flow',
        loadComponent: () => import('./features/cash-flow/components/cash-flow-main/cash-flow-main.component').then(m => m.CashFlowMainComponent)
      },
      {
        path: 'accounts-payable',
        loadComponent: () => import('./features/accounts-payable/components/payable-list/payable-list.component').then(m => m.PayableListComponent)
      },
      {
        path: 'accounts-receivable',
        loadComponent: () => import('./features/accounts-receivable/components/receivable-list/receivable-list.component').then(m => m.ReceivableListComponent)
      },
      {
        path: 'inventory',
        loadComponent: () => import('./features/inventory/components/inventory-main/inventory-main.component').then(m => m.InventoryMainComponent)
      },
      {
        path: 'purchases',
        loadComponent: () => import('./features/purchases/components/purchase-list/purchase-list.component').then(m => m.PurchaseListComponent)
      },
      {
        path: 'recipe',
        loadComponent: () => import('./features/recipe/components/recipe-list/recipe-list.component').then(m => m.RecipeListComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/components/reports-main/reports-main.component').then(m => m.ReportsMainComponent)
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
