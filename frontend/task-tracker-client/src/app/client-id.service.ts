import { Injectable } from '@angular/core';

const STORAGE_KEY = 'taskTracker.clientId';
const UUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

// An anonymous random ID, generated once per browser and used only to scope that browser's tasks.
@Injectable({ providedIn: 'root' })
export class ClientIdService {
  readonly id = this.loadOrCreate();

  private loadOrCreate(): string {
    try {
      const existing = localStorage.getItem(STORAGE_KEY);
      if (existing && UUID_PATTERN.test(existing)) return existing;
      const created = crypto.randomUUID();
      localStorage.setItem(STORAGE_KEY, created);
      return created;
    } catch {
      // Storage is blocked (some private modes): the list then lasts only for this page load.
      return crypto.randomUUID();
    }
  }
}
