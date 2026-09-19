import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from './task.service';
import { TaskItem } from './task.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h1>Task Tracker</h1>

    <div class="new-task">
      <input
        [(ngModel)]="newTitle"
        placeholder="What needs doing?"
        (keyup.enter)="addTask()"
      />
      <button (click)="addTask()" [disabled]="!newTitle.trim()">Add</button>
    </div>

    <ul class="task-list">
      <li *ngFor="let task of tasks" [class.done]="task.isComplete">
        <label>
          <input
            type="checkbox"
            [checked]="task.isComplete"
            (change)="toggleComplete(task)"
          />
          {{ task.title }}
        </label>
        <button class="delete" (click)="removeTask(task)">✕</button>
      </li>
    </ul>

    <p *ngIf="!tasks.length">No tasks yet — add one above.</p>
  `,
  styles: [
    `
      .new-task {
        display: flex;
        gap: 8px;
        margin-bottom: 16px;
      }
      .new-task input {
        flex: 1;
        padding: 8px;
      }
      .task-list {
        list-style: none;
        padding: 0;
      }
      .task-list li {
        display: flex;
        justify-content: space-between;
        align-items: center;
        background: white;
        padding: 10px 12px;
        margin-bottom: 6px;
        border-radius: 6px;
      }
      .task-list li.done label {
        text-decoration: line-through;
        color: #888;
      }
      .delete {
        background: none;
        border: none;
        color: #c00;
        cursor: pointer;
      }
    `,
  ],
})
export class AppComponent implements OnInit {
  tasks: TaskItem[] = [];
  newTitle = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getAll().subscribe((tasks) => (this.tasks = tasks));
  }

  addTask(): void {
    const title = this.newTitle.trim();
    if (!title) return;
    this.taskService.create({ title, isComplete: false }).subscribe(() => {
      this.newTitle = '';
      this.loadTasks();
    });
  }

  toggleComplete(task: TaskItem): void {
    const updated = { ...task, isComplete: !task.isComplete };
    this.taskService.update(task.id, updated).subscribe(() => this.loadTasks());
  }

  removeTask(task: TaskItem): void {
    this.taskService.delete(task.id).subscribe(() => this.loadTasks());
  }
}
