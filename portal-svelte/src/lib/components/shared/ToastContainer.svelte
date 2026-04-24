<!--
  ToastContainer.svelte
  Replaces: Blazored.Toast <BlazoredToasts>
  DaisyUI toast notifications with auto-dismiss
-->
<script lang="ts">
	import { toasts, type Toast } from '$lib/stores/toast';

	const typeMap: Record<Toast['type'], string> = {
		success: 'alert-success',
		error: 'alert-error',
		warning: 'alert-warning',
		info: 'alert-info'
	};

	const iconMap: Record<Toast['type'], string> = {
		success: '✓',
		error: '✕',
		warning: '⚠',
		info: 'ℹ'
	};
</script>

<div class="toast toast-end toast-bottom z-50">
	{#each $toasts as toast (toast.id)}
		<div class="alert {typeMap[toast.type]} shadow-lg">
			<span class="text-lg font-bold">{iconMap[toast.type]}</span>
			<span>{toast.message}</span>
			<button
				class="btn btn-ghost btn-xs"
				onclick={() => toasts.remove(toast.id)}
				aria-label="Cerrar notificación"
			>
				✕
			</button>
		</div>
	{/each}
</div>
