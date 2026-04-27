<script lang="ts">
  import { toasts } from '$lib/stores/toast';

  const typeClasses: Record<string, string> = {
    success: 'alert-success',
    error: 'alert-error',
    warning: 'alert-warning',
    info: 'alert-info'
  };
</script>

<div class="toast toast-end toast-bottom z-50" aria-live="polite">
  {#each $toasts as toast (toast.id)}
    <div class="alert {typeClasses[toast.type] ?? 'alert-info'} shadow-lg">
      <span>{toast.message}</span>
      <button
        class="btn btn-sm btn-ghost"
        onclick={() => toasts.remove(toast.id)}
        aria-label="Cerrar notificación"
      >✕</button>
    </div>
  {/each}
</div>
