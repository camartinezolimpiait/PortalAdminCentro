<!--
  AlertMessage.svelte
  Replaces: Shared/AlertMessage.razor
  DaisyUI alert component with type variants
-->
<script lang="ts">
	let {
		message = '',
		type = ''
	}: {
		message?: string;
		type?: 'success' | 'error' | 'warning' | 'info' | '';
	} = $props();

	const alertClass: Record<string, string> = {
		success: 'alert-success',
		error: 'alert-error',
		warning: 'alert-warning',
		info: 'alert-info',
		danger: 'alert-error'
	};
</script>

{#if message && type}
	<div class="alert {alertClass[type] || 'alert-info'} mt-3" role="alert">
		<span>{message}</span>
	</div>
{/if}
