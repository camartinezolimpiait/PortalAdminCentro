// ===================================================================
// Toast / Notification Store
// Replaces: Blazored.Toast (IToastService)
// ===================================================================

import { writable } from 'svelte/store';

export interface Toast {
	id: number;
	message: string;
	type: 'success' | 'error' | 'warning' | 'info';
	timeout: number;
}

function createToastStore() {
	const { subscribe, update } = writable<Toast[]>([]);
	let nextId = 0;

	function add(message: string, type: Toast['type'] = 'info', timeout = 5000) {
		const id = nextId++;
		update((toasts) => [...toasts, { id, message, type, timeout }]);
		if (timeout > 0) {
			setTimeout(() => remove(id), timeout);
		}
	}

	function remove(id: number) {
		update((toasts) => toasts.filter((t) => t.id !== id));
	}

	return {
		subscribe,
		success: (msg: string, timeout?: number) => add(msg, 'success', timeout),
		error: (msg: string, timeout?: number) => add(msg, 'error', timeout),
		warning: (msg: string, timeout?: number) => add(msg, 'warning', timeout),
		info: (msg: string, timeout?: number) => add(msg, 'info', timeout),
		remove
	};
}

export const toasts = createToastStore();
