import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Table, CreateTableRequest, UpdateTableRequest, UpdateTableStatusRequest } from '../../shared/models/table.model';

@Injectable({
  providedIn: 'root'
})
export class TableService {
  private readonly apiUrl = 'http://localhost:5020/api/tables';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Table[]> {
    return this.http.get<Table[]>(this.apiUrl);
  }

  getById(id: string): Observable<Table> {
    return this.http.get<Table>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTableRequest): Observable<Table> {
    return this.http.post<Table>(this.apiUrl, request);
  }

  update(id: string, request: UpdateTableRequest): Observable<Table> {
    return this.http.put<Table>(`${this.apiUrl}/${id}`, request);
  }

  updateStatus(id: string, request: UpdateTableStatusRequest): Observable<Table> {
    return this.http.patch<Table>(`${this.apiUrl}/${id}/status`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
