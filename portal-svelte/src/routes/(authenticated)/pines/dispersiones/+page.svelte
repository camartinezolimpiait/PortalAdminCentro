<script lang="ts">
import DataTable from '$lib/components/shared/DataTable.svelte';
let { data } = $props();
const columns = [
{ key: 'fechaDispersion', label: 'Fecha' },
{ key: 'valor', label: 'Valor', render: (v: unknown) => `$${Number(v).toLocaleString('es-CO')}` },
{ key: 'estado', label: 'Estado' },
{ key: 'referencia', label: 'Referencia' }
];
const tableData = $derived(Array.isArray(data.dispersiones) ? data.dispersiones.map((d: Record<string, unknown>) => ({...d})) : []);
</script>
<svelte:head><title>Dispersiones</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Dispersiones</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>
{#if data.error}<div class="alert alert-error">{data.error}</div>{/if}
<DataTable data={tableData} {columns} emptyMessage="No se encontraron dispersiones" />
</div>
