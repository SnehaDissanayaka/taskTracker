import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { TaskService } from './task.service';
import { TaskItem } from './task.model';

@Injectable({ providedIn: 'root' })
export class TaskStore {
  private readonly _tasks = signal<TaskItem[]>([]);

  readonly tasks = this._tasks.asReadonly();
  readonly remaining = computed(
    () => this._tasks().filter((t) => !t.isComplete).length,
  );

  constructor(private api: TaskService) {}

  load(): void {
    this.api.getAll().subscribe((tasks) => this._tasks.set(tasks));
  }

  add(title: string): Observable<TaskItem> {
    return this.api.create({ title }).pipe(tap(() => this.load()));
  }

  toggle(task: TaskItem): Observable<void> {
    return this.api
      .update(task.id, {
        title: task.title,
        description: task.description,
        isComplete: !task.isComplete,
      })
      .pipe(tap(() => this.load()));
  }

  remove(task: TaskItem): Observable<void> {
    return this.api.delete(task.id).pipe(tap(() => this.load()));
  }
}
