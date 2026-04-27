<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    open: boolean;
    title?: string;
    onclose: () => void;
    children: Snippet;
  }

  let { open, title = '', onclose, children }: Props = $props();

  function handleKeydown(e: KeyboardEvent) {
    if (e.key === 'Escape') onclose();
  }
</script>

{#if open}
  <!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
  <dialog class="modal modal-open" role="dialog" aria-modal="true" aria-label={title} onkeydown={handleKeydown}>
    <div class="modal-box">
      {#if title}
        <h3 class="font-bold text-lg mb-4">{title}</h3>
      {/if}
      <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2" onclick={onclose} aria-label="Cerrar">✕</button>
      {@render children()}
    </div>
    <!-- svelte-ignore a11y_click_events_have_key_events a11y_no_static_element_interactions -->
    <div class="modal-backdrop" role="presentation" onclick={onclose}></div>
  </dialog>
{/if}
