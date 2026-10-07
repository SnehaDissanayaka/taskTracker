import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateTaskRequest, TaskItem, UpdateTaskRequest } from './task.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private baseUrl = `${environment.apiBaseUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.baseUrl);
  }

  create(task: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, task);
  }

  update(id: number, task: UpdateTaskRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, task);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
