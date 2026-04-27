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
    <div class="modal-backdrop" onclick={onclose}></div>
  </dialog>
{/if}
