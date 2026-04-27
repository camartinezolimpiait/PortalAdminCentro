<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    loading?: boolean;
    text?: string;
    type?: 'submit' | 'button' | 'reset';
    disabled?: boolean;
    variant?: string;
    onclick?: () => void;
    children?: Snippet;
  }

  let { loading = false, text = '', type = 'button', disabled = false, variant = 'btn-primary', onclick, children }: Props = $props();
</script>

<button
  {type}
  class="btn {variant}"
  disabled={disabled || loading}
  {onclick}
>
  {#if loading}
    <span class="loading loading-spinner loading-sm"></span>
  {/if}
  {#if children}
    {@render children()}
  {:else}
    {text}
  {/if}
</button>
