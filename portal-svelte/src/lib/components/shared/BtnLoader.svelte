<!--
  BtnLoader.svelte
  Replaces: Shared/BtnLoader.razor
  DaisyUI loading button component
-->
<script lang="ts">
	let {
		showLoader = false,
		text = 'Procesando...',
		class: className = 'btn btn-primary w-full'
	}: {
		showLoader?: boolean;
		text?: string;
		class?: string;
	} = $props();
</script>

{#if showLoader}
	<button class={className} type="button" disabled>
		<span class="loading loading-spinner loading-sm"></span>
		{text}
	</button>
{/if}
