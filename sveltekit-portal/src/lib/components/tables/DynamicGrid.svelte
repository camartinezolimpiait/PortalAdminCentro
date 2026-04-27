<script lang="ts">
  interface Column {
    field: string;
    header: string;
    sortable?: boolean;
    type?: string;
  }

  interface Props {
    columns: Column[];
    data: Record<string, unknown>[];
    loading?: boolean;
    emptyMessage?: string;
  }

  let { columns, data, loading = false, emptyMessage = 'No hay datos disponibles' }: Props = $props();
  let sortField = $state('');
  let sortAsc = $state(true);

  const sortedData = $derived(() => {
    if (!sortField) return data;
    return [...data].sort((a, b) => {
      const va = a[sortField];
      const vb = b[sortField];
      if (va == null || vb == null) return 0;
      const cmp = String(va).localeCompare(String(vb), 'es', { numeric: true });
      return sortAsc ? cmp : -cmp;
    });
  });

  function toggleSort(field: string) {
    if (sortField === field) {
      sortAsc = !sortAsc;
    } else {
      sortField = field;
      sortAsc = true;
    }
  }
</script>

<div class="overflow-x-auto">
  <table class="table table-zebra w-full">
    <thead>
      <tr>
        {#each columns as col}
          <th>
            {#if col.sortable}
              <button class="flex items-center gap-1 hover:text-primary" onclick={() => toggleSort(col.field)}>
                {col.header}
                {#if sortField === col.field}
                  <span>{sortAsc ? '▲' : '▼'}</span>
                {/if}
              </button>
            {:else}
              {col.header}
            {/if}
          </th>
        {/each}
      </tr>
    </thead>
    <tbody>
      {#if loading}
        {#each Array(5) as _}
          <tr>
            {#each columns as _col}
              <td><div class="skeleton h-4 w-full"></div></td>
            {/each}
          </tr>
        {/each}
      {:else if sortedData().length === 0}
        <tr>
          <td colspan={columns.length} class="text-center py-8 text-base-content/50">
            {emptyMessage}
          </td>
        </tr>
      {:else}
        {#each sortedData() as row}
          <tr>
            {#each columns as col}
              <td>{row[col.field] ?? ''}</td>
            {/each}
          </tr>
        {/each}
      {/if}
    </tbody>
  </table>
</div>
