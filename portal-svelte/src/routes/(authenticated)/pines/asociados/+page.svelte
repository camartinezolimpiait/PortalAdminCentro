<script lang="ts">
import { enhance } from '$app/forms';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
let { form } = $props();
</script>
<svelte:head><title>Pines Asociados</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Pines Asociados</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>

<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" action="?/search" use:enhance>
<div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
<select name="tipoDocumento" class="select select-bordered" required>
<option value="" disabled selected>Tipo Documento</option>
<option value="CC">Cédula de Ciudadanía</option>
<option value="CE">Cédula de Extranjería</option>
<option value="PA">Pasaporte</option>
<option value="TI">Tarjeta de Identidad</option>
</select>
<input type="text" name="numeroDocumento" class="input input-bordered" placeholder="Número de documento" required />
<button type="submit" class="btn btn-primary">Consultar</button>
</div>
</form>

{#if form?.error}
<AlertMessage message={form.error} type="error" />
{/if}

{#if form?.results}
<div class="mt-4 overflow-x-auto">
<pre class="bg-base-200 p-4 rounded-lg text-sm">{JSON.stringify(form.results, null, 2)}</pre>
</div>
{/if}
</div>
</div>
</div>
