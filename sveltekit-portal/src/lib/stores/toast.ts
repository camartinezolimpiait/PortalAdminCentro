import { writable } from 'svelte/store';

interface Toast {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
}

let nextId = 0;

function createToastStore() {
  const { subscribe, update } = writable<Toast[]>([]);

  return {
    subscribe,
    add(type: Toast['type'], message: string, timeout = 10000) {
      const id = nextId++;
      update((toasts) => [...toasts, { id, type, message }]);
      if (timeout > 0) {
        setTimeout(() => {
          update((toasts) => toasts.filter((t) => t.id !== id));
        }, timeout);
      }
      return id;
    },
    remove(id: number) {
      update((toasts) => toasts.filter((t) => t.id !== id));
    }
  };
}

export const toasts = createToastStore();
