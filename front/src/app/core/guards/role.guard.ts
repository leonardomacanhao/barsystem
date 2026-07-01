import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRoles = route.data['roles'] as string[];
    const user = this.authService.getCurrentUser();
    
    if (!user) {
      this.router.navigate(['/login']);
      return false;
    }
    
    if (!requiredRoles || requiredRoles.length === 0) {
      return true;
    }
    
    if (requiredRoles.includes(user.role)) {
      return true;
    }
    
    this.toastr.error('Você não tem permissão para acessar esta página', 'Acesso Negado');
    this.router.navigate(['/']);
    return false;
  }
}
