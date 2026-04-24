<!--
  NavMenuPrincipal.svelte
  Replaces: Shared/NavMenuPrincipal.razor
  Main horizontal navigation with DaisyUI menu
-->
<script lang="ts">
	import type { MenuItem } from '$lib/types/models';

	let {
		menuItems = [],
		plataforma = ''
	}: {
		menuItems?: MenuItem[];
		plataforma?: string;
	} = $props();

	const sortedItems = $derived(
		[...menuItems].sort((a, b) => (a.pagina || '').localeCompare(b.pagina || ''))
	);
</script>

{#if menuItems.length === 0}
	<ul class="menu menu-horizontal gap-1">
		{#each Array(4) as _}
			<li>
				<div class="skeleton h-4 w-24 opacity-50"></div>
			</li>
		{/each}
	</ul>
{:else}
	<ul class="menu menu-horizontal gap-1 flex-nowrap">
		{#each sortedItems as item}
			<li>
				<a
					href={item.ruta || '#'}
					class="btn btn-ghost btn-sm text-sm whitespace-nowrap"
				>
					{item.pagina || item.menuName || item.ruta}
				</a>
			</li>
		{/each}
	</ul>
{/if}
