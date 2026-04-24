<!--
  ConfirmModal.svelte
  Replaces: Bootstrap modals used across Blazor pages
  DaisyUI modal with confirm/cancel actions
-->
<script lang="ts">
	let {
		open = false,
		title = 'Confirmar',
		message = '¿Está seguro de realizar esta acción?',
		confirmText = 'Confirmar',
		cancelText = 'Cancelar',
		confirmClass = 'btn-primary',
		onConfirm,
		onCancel
	}: {
		open?: boolean;
		title?: string;
		message?: string;
		confirmText?: string;
		cancelText?: string;
		confirmClass?: string;
		onConfirm?: () => void;
		onCancel?: () => void;
	} = $props();
</script>

<dialog class="modal" class:modal-open={open}>
	<div class="modal-box">
		<h3 class="text-lg font-bold">{title}</h3>
		<p class="py-4">{message}</p>
		<div class="modal-action">
			<button class="btn" onclick={onCancel}>{cancelText}</button>
			<button class="btn {confirmClass}" onclick={onConfirm}>{confirmText}</button>
		</div>
	</div>
	<form method="dialog" class="modal-backdrop">
		<button onclick={onCancel}>close</button>
	</form>
</dialog>
