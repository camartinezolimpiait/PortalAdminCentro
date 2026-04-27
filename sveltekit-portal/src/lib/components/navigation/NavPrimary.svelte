<script lang="ts">
  import type { MenuItem } from '$lib/types/auth';

  interface Props {
    items: MenuItem[];
  }

  let { items }: Props = $props();

  const sortedItems = $derived(
    [...items].sort((a, b) => (a.pagina ?? '').localeCompare(b.pagina ?? '', 'es'))
  );
</script>

<ul class="menu menu-horizontal px-1" role="menubar">
  {#if items.length === 0}
    {#each Array(4) as _}
      <li><div class="skeleton h-4 w-20"></div></li>
    {/each}
  {:else}
    {#each sortedItems as item}
      <li role="none">
        <a href={item.ruta || '#'} role="menuitem">{item.pagina}</a>
      </li>
    {/each}
  {/if}
</ul>
