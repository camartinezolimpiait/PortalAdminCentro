<script lang="ts">
import DataTable from '$lib/components/shared/DataTable.svelte';
let { data } = $props();
const columns = [
{ key: 'fechaCita', label: 'Fecha' },
{ key: 'horaCita', label: 'Hora' },
{ key: 'nombres', label: 'Nombres' },
{ key: 'apellidos', label: 'Apellidos' },
{ key: 'tipoDocumento', label: 'Tipo Doc.' },
{ key: 'numeroDocumento', label: 'Número Doc.' },
{ key: 'tramite', label: 'Trámite' },
{ key: 'categoria', label: 'Categoría' },
{ key: 'estado', label: 'Estado' }
];
const tableData = $derived(data.citas.map((c: Record<string, unknown>) => ({...c})));
</script>
<svelte:head><title>Agenda</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">📅 Agenda</h1>
<a href="/agendamiento" class="btn btn-ghost btn-sm">← Volver</a>
</div>
{#if data.error}<div class="alert alert-error">{data.error}</div>{/if}
<DataTable data={tableData} {columns} emptyMessage="No hay citas para hoy" />
</div>
