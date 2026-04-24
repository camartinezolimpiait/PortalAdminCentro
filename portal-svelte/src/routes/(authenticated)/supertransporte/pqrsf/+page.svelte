<script lang="ts">
import { enhance } from '$app/forms';
import DataTable from '$lib/components/shared/DataTable.svelte';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
let { data, form } = $props();
let showForm = $state(false);
const columns = [
{ key: 'tipo', label: 'Tipo' },
{ key: 'asunto', label: 'Asunto' },
{ key: 'fechaRadicacion', label: 'Fecha' },
{ key: 'estado', label: 'Estado' }
];
const tableData = $derived(data.pqrsf.map((p: Record<string, unknown>) => ({...p})));
</script>
<svelte:head><title>PQRSF - SuperTransporte</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">PQRSF</h1>
<div class="flex gap-2">
<button class="btn btn-primary btn-sm" onclick={() => showForm = !showForm}>
{showForm ? 'Cancelar' : '+ Nueva'}
</button>
<a href="/supertransporte" class="btn btn-ghost btn-sm">← Volver</a>
</div>
</div>
{#if showForm}
<div class="card bg-base-100 shadow">
<div class="card-body">
<h2 class="card-title">Nueva PQRSF</h2>
<form method="POST" action="?/create" use:enhance>
<div class="grid grid-cols-1 gap-4">
<select name="tipo" class="select select-bordered" required>
<option value="" disabled selected>Tipo</option>
<option value="Peticion">Petición</option>
<option value="Queja">Queja</option>
<option value="Reclamo">Reclamo</option>
<option value="Sugerencia">Sugerencia</option>
<option value="Felicitacion">Felicitación</option>
</select>
<input name="asunto" type="text" class="input input-bordered" placeholder="Asunto" required />
<textarea name="descripcion" class="textarea textarea-bordered" placeholder="Descripción" rows="4" required></textarea>
<button type="submit" class="btn btn-primary">Enviar</button>
</div>
</form>
{#if form?.error}<AlertMessage message={form.error} type="error" />{/if}
{#if form?.success}<AlertMessage message="PQRSF creada exitosamente" type="success" />{/if}
</div>
</div>
{/if}
<DataTable data={tableData} {columns} emptyMessage="No hay PQRSF registradas" />
</div>
