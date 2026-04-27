<script lang="ts">
  import DynamicGrid from '$lib/components/tables/DynamicGrid.svelte';

  const columns = [
    { field: 'numeroFactura', header: 'No. Factura', sortable: true },
    { field: 'fecha', header: 'Fecha', sortable: true },
    { field: 'cliente', header: 'Cliente', sortable: true },
    { field: 'nit', header: 'NIT', sortable: true },
    { field: 'total', header: 'Total', sortable: true },
    { field: 'estado', header: 'Estado', sortable: true }
  ];
</script>

<h1 class="text-2xl font-bold mb-4">Consultar Facturación</h1>
<DynamicGrid {columns} data={[]} emptyMessage="No se encontraron facturas" />
