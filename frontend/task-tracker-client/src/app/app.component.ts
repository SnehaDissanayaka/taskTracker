import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule } from '@angular/material/list';
import { TaskService } from './task.service';
import { TaskItem } from './task.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatToolbarModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatListModule,
  ],
  template: `
    <mat-toolbar color="primary">
      <mat-icon>task_alt</mat-icon>
      <span class="title">Task Tracker</span>
      <span class="spacer"></span>
      <span class="count" *ngIf="tasks.length">
        {{ remaining }} of {{ tasks.length }} left
      </span>
    </mat-toolbar>

    <main>
      <mat-card>
        <mat-card-content>
          <div class="new-task">
            <mat-form-field appearance="outline" subscriptSizing="dynamic">
              <mat-label>What needs doing?</mat-label>
              <input
                matInput
                [(ngModel)]="newTitle"
                (keyup.enter)="addTask()"
              />
            </mat-form-field>
            <button
              mat-flat-button
              color="primary"
              (click)="addTask()"
              [disabled]="!newTitle.trim()"
            >
              <mat-icon>add</mat-icon>
              Add
            </button>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="list-card">
        <mat-list *ngIf="tasks.length; else empty">
          <mat-list-item *ngFor="let task of tasks">
            <div class="row">
              <mat-checkbox
                color="primary"
                [checked]="task.isComplete"
                (change)="toggleComplete(task)"
              >
                <span [class.done-text]="task.isComplete">{{ task.title }}</span>
              </mat-checkbox>
              <button
                mat-icon-button
                color="warn"
                aria-label="Delete task"
                (click)="removeTask(task)"
              >
                <mat-icon>delete_outline</mat-icon>
              </button>
            </div>
          </mat-list-item>
        </mat-list>
        <ng-template #empty>
          <div class="empty">
            <mat-icon>checklist</mat-icon>
            <p>No tasks yet — add one above.</p>
          </div>
        </ng-template>
      </mat-card>
    </main>
  `,
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  tasks: TaskItem[] = [];
  newTitle = '';

  constructor(private taskService: TaskService) {}

  get remaining(): number {
    return this.tasks.filter((t) => !t.isComplete).length;
  }

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
