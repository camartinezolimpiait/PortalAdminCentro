<script lang="ts">
import { enhance } from '$app/forms';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
let { data, form } = $props();
</script>
<svelte:head><title>Búsqueda de Pines</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Búsqueda de Pines</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>

<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" action="?/search" use:enhance>
<div class="flex flex-col sm:flex-row gap-3">
<select name="searchType" class="select select-bordered">
<option value="pin">Por PIN</option>
<option value="documento">Por Documento</option>
</select>
<input type="text" name="searchValue" class="input input-bordered flex-1" placeholder="Ingrese el valor a buscar" required />
<button type="submit" class="btn btn-primary">Buscar</button>
</div>
</form>

{#if form?.error}
<AlertMessage message={form.error} type="error" />
{/if}

{#if form?.results}
<div class="mt-4">
<pre class="bg-base-200 p-4 rounded-lg text-sm overflow-x-auto">{JSON.stringify(form.results, null, 2)}</pre>
</div>
{/if}
</div>
</div>
</div>
