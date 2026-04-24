<!--
  NavMenuPortal.svelte
  Replaces: Shared/NavMenuPortal.razor
  Sidebar/drawer navigation with collapsible sub-menus
-->
<script lang="ts">
	import type { MenuItem } from '$lib/types/models';
	import { page } from '$app/stores';

	let {
		menuItems = [],
		subMenuItems = []
	}: {
		menuItems?: MenuItem[];
		subMenuItems?: MenuItem[];
	} = $props();

	let expandedMenuId: number | null = $state(null);

	function toggleSubMenu(menuId: number) {
		expandedMenuId = expandedMenuId === menuId ? null : menuId;
	}

	function getSubItems(parentId: number): MenuItem[] {
		return subMenuItems.filter((m) => m.padre === parentId);
	}
</script>

{#if menuItems.length === 0}
	<div class="p-4">
		<p class="text-base-content/50">Cargando...</p>
	</div>
{:else}
	<ul class="menu bg-base-200 rounded-box w-full">
		{#each menuItems as item}
			{@const subItems = getSubItems(item.id)}
			<li>
				{#if subItems.length > 0}
					<details open={expandedMenuId === item.id}>
						<summary
							class="font-medium"
							onclick={() => toggleSubMenu(item.id)}
						>
							{#if item.iconoNombre}
								<span class={item.iconoNombre} aria-hidden="true"></span>
							{/if}
							{item.ruta || item.pagina}
						</summary>
						<ul>
							{#each subItems as subItem}
								<li>
									<a
										href={subItem.ruta || '#'}
										class:active={$page.url.pathname === subItem.ruta}
									>
										{#if subItem.iconoNombre}
											<span class={subItem.iconoNombre} aria-hidden="true"></span>
										{/if}
										{subItem.ruta || subItem.pagina}
									</a>
								</li>
							{/each}
						</ul>
					</details>
				{:else}
					<a
						href={item.ruta || '#'}
						class:active={$page.url.pathname === item.ruta}
					>
						{#if item.iconoNombre}
							<span class={item.iconoNombre} aria-hidden="true"></span>
						{/if}
						{item.ruta || item.pagina}
					</a>
				{/if}
			</li>
		{/each}
	</ul>
{/if}
