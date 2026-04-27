<script lang="ts">
  interface Props {
    type?: 'info' | 'success' | 'warning' | 'error';
    message: string;
    dismissible?: boolean;
  }

  let { type = 'info', message, dismissible = false }: Props = $props();
  let visible = $state(true);

  const typeClasses: Record<string, string> = {
    info: 'alert-info',
    success: 'alert-success',
    warning: 'alert-warning',
    error: 'alert-error'
  };
</script>

{#if visible}
  <div class="alert {typeClasses[type]} mb-4" role="alert">
    <span>{message}</span>
    {#if dismissible}
      <button class="btn btn-sm btn-ghost" onclick={() => (visible = false)} aria-label="Cerrar">✕</button>
    {/if}
  </div>
{/if}
