// ===================================================================
// Loading Store
// Replaces: SpinLoader/SpinKit loading indicators from Blazor
// ===================================================================

import { writable } from 'svelte/store';

function createLoadingStore() {
	const { subscribe, set, update } = writable(false);
	let count = 0;

	return {
		subscribe,
		start: () => {
			count++;
			set(true);
		},
		stop: () => {
			count = Math.max(0, count - 1);
			if (count === 0) set(false);
		},
		reset: () => {
			count = 0;
			set(false);
		}
	};
}

export const loading = createLoadingStore();
