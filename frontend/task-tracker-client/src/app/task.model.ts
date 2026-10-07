export interface TaskItem {
  id: number;
  title: string;
  description?: string;
  isComplete: boolean;
  createdAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  isComplete: boolean;
}
