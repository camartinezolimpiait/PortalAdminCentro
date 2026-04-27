<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    label: string;
    name: string;
    error?: string;
    required?: boolean;
    children: Snippet;
  }

  let { label, name, error = '', required = false, children }: Props = $props();
</script>

<div class="form-control w-full">
  <label class="label" for={name}>
    <span class="label-text">
      {label}
      {#if required}<span class="text-error">*</span>{/if}
    </span>
  </label>
  {@render children()}
  {#if error}
    <label class="label" for={name}>
      <span class="label-text-alt text-error">{error}</span>
    </label>
  {/if}
</div>
