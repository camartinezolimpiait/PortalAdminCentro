<script lang="ts">
  import type { MenuItem } from '$lib/types/auth';

  interface Props {
    items: MenuItem[];
    onNavigate?: () => void;
  }

  let { items, onNavigate }: Props = $props();
  let expandedId = $state<number | null>(null);

  const parents = $derived(items.filter((m) => !items.some((c) => c.id === m.padre && c.id !== m.id) && m.padre === 0 || !items.find((p) => p.id === m.padre)));

  function getChildren(parentId: number): MenuItem[] {
    return items.filter((m) => m.padre === parentId && m.id !== parentId);
  }

  function toggleExpand(id: number) {
    expandedId = expandedId === id ? null : id;
  }
</script>

<ul class="menu w-full" role="navigation" aria-label="Menú de navegación">
  {#each items as item}
    {@const children = getChildren(item.id)}
    {#if item.padre === 0 || !items.find((p) => p.id === item.padre)}
      <li>
        {#if children.length > 0}
          <details open={expandedId === item.id}>
            <summary
              class="font-medium"
              onclick={(e) => { e.preventDefault(); toggleExpand(item.id); }}
            >
              {#if item.iconoNombre}
                <span class={item.iconoNombre} aria-hidden="true"></span>
              {/if}
              {item.ruta || item.pagina}
            </summary>
            <ul>
              {#each children as child}
                <li>
                  <a href={child.ruta || '#'} onclick={() => onNavigate?.()}>
                    {child.ruta || child.pagina}
                  </a>
                </li>
              {/each}
            </ul>
          </details>
        {:else}
          <a href={item.ruta || '#'} onclick={() => onNavigate?.()}>
            {#if item.iconoNombre}
              <span class={item.iconoNombre} aria-hidden="true"></span>
            {/if}
            {item.ruta || item.pagina}
          </a>
        {/if}
      </li>
    {/if}
  {/each}
</ul>
