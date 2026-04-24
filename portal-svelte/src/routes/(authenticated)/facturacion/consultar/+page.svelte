<script lang="ts">
import { enhance } from '$app/forms';
import DataTable from '$lib/components/shared/DataTable.svelte';
let { data, form } = $props();
const columns = [
{ key: 'numero', label: 'Número' },
{ key: 'fecha', label: 'Fecha' },
{ key: 'razonSocial', label: 'Razón Social' },
{ key: 'nit', label: 'NIT' },
{ key: 'valorTotal', label: 'Valor', render: (v: unknown) => `$${Number(v).toLocaleString('es-CO')}` },
{ key: 'estado', label: 'Estado' }
];
const tableData = $derived((form?.facturas || data.facturas || []).map((f: Record<string, unknown>) => ({...f})));
</script>
<svelte:head><title>Consultar Facturación</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">Consultar Facturación</h1>
<a href="/facturacion" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" action="?/search" use:enhance>
<div class="flex flex-col sm:flex-row gap-3 mb-4">
<input type="date" name="fechaInicio" class="input input-bordered" />
<input type="date" name="fechaFin" class="input input-bordered" />
<select name="estado" class="select select-bordered">
<option value="">Todos</option>
<option value="Pendiente">Pendiente</option>
<option value="Enviada">Enviada</option>
<option value="Aceptada">Aceptada</option>
<option value="Rechazada">Rechazada</option>
</select>
<button type="submit" class="btn btn-primary">Buscar</button>
</div>
</form>
{#if form?.error}<div class="alert alert-error">{form.error}</div>{/if}
<DataTable data={tableData} {columns} emptyMessage="No se encontraron facturas" />
</div>
</div>
</div>
