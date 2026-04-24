<script lang="ts">
import DataTable from '$lib/components/shared/DataTable.svelte';
import { goto } from '$app/navigation';
let { data } = $props();
const columns = [
{ key: 'pin', label: 'PIN' },
{ key: 'fechaDevolucion', label: 'Fecha Devolución' },
{ key: 'valor', label: 'Valor', render: (v: unknown) => `$${Number(v).toLocaleString('es-CO')}` },
{ key: 'motivo', label: 'Motivo' },
{ key: 'estado', label: 'Estado' }
];
const tableData = $derived(Array.isArray(data.devoluciones) ? data.devoluciones.map((d: Record<string, unknown>) => ({...d})) : []);
</script>
<svelte:head><title>Devoluciones de Pines</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Devoluciones</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>
{#if data.error}<div class="alert alert-error">{data.error}</div>{/if}
<DataTable data={tableData} {columns} emptyMessage="No se encontraron devoluciones" />
</div>
