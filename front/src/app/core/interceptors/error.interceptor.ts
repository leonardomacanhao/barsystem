import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ocorreu um erro inesperado';
        
        if (error.status === 401) {
          errorMessage = 'Sessão expirada. Faça login novamente.';
          this.router.navigate(['/login']);
        } else if (error.status === 403) {
          errorMessage = 'Você não tem permissão para acessar este recurso.';
        } else if (error.status === 404) {
          errorMessage = 'Recurso não encontrado.';
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        }
        
        this.toastr.error(errorMessage, 'Erro');
        return throwError(() => error);
      })
    );
  }
}
