<script lang="ts">
  import DynamicGrid from '$lib/components/tables/DynamicGrid.svelte';

  interface Props {
    data: { estado: string; pines: Record<string, unknown>[] };
  }

  let { data }: Props = $props();

  const tabs = [
    { id: 'activos', label: 'Activos' },
    { id: 'usados', label: 'Usados' },
    { id: 'dispersiones', label: 'Dispersiones' },
    { id: 'devoluciones', label: 'Devoluciones' },
    { id: 'busqueda', label: 'Búsqueda' },
    { id: 'pinesasociados', label: 'Asociados' },
    { id: 'certificadoingreso', label: 'Certificado' }
  ] as const;

  const activeTab = $derived(data.estado);

  const columns = [
    { field: 'pin', header: 'PIN', sortable: true },
    { field: 'estado', header: 'Estado', sortable: true },
    { field: 'fechaCreacion', header: 'Fecha Creación', sortable: true },
    { field: 'fechaVencimiento', header: 'Vencimiento', sortable: true },
    { field: 'centro', header: 'Centro', sortable: true },
    { field: 'categoria', header: 'Categoría', sortable: true },
    { field: 'valorPin', header: 'Valor', sortable: true }
  ];
</script>

<h1 class="text-2xl font-bold mb-4">Gestión de PINes</h1>

<div role="tablist" class="tabs tabs-bordered mb-6">
  {#each tabs as tab}
    <a
      role="tab"
      class="tab {activeTab === tab.id ? 'tab-active' : ''}"
      href="/pines?estado={tab.id}"
    >
      {tab.label}
    </a>
  {/each}
</div>

<DynamicGrid {columns} data={data.pines} emptyMessage="No se encontraron PINes" />
