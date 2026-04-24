<!--
  Pines Activos
  Replaces: Pages/Pines/Activos/Activos.razor
-->
<script lang="ts">
import DataTable from '$lib/components/shared/DataTable.svelte';
import { goto } from '$app/navigation';

let { data } = $props();

const columns = [
{ key: 'pin', label: 'PIN' },
{ key: 'fechaCompra', label: 'Fecha Compra' },
{ key: 'fechaExpiracion', label: 'Fecha Expiración' },
{ key: 'categoria', label: 'Categoría' },
{ key: 'tramite', label: 'Trámite' },
{ key: 'tipoDocumento', label: 'Tipo Doc.' },
{ key: 'numeroDocumento', label: 'Número Doc.' },
{ key: 'nombres', label: 'Nombres' },
{ key: 'apellidos', label: 'Apellidos' },
{
key: 'valor',
label: 'Valor',
render: (v: unknown) => `$${Number(v).toLocaleString('es-CO')}`
}
];

function handlePageChange(page: number) {
goto(`/pines/activos?page=${page}&pageSize=${data.pageSize}`);
}

const tableData = $derived(
(data.pines?.pines || []).map((p) => ({ ...p } as Record<string, unknown>))
);
const totalPages = $derived(
Math.ceil((data.pines?.totalRegistros || 0) / data.pageSize)
);
</script>

<svelte:head>
<title>Pines Activos</title>
</svelte:head>

<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Pines Activos</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>

{#if data.error}
<div class="alert alert-error">{data.error}</div>
{/if}

<DataTable
data={tableData}
{columns}
currentPage={data.currentPage}
{totalPages}
onPageChange={handlePageChange}
emptyMessage="No se encontraron pines activos"
/>
</div>
