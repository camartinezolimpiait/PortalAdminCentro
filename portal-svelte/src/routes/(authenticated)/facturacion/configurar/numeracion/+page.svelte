<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>numeracion - Facturación</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">numeracion</h2>
<a href="/facturacion/configurar" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<p class="mb-4">Configuración de numeracion de facturación electrónica.</p>
<div class="card-actions justify-end mt-4">
<button type="submit" class="btn btn-primary">Guardar</button>
</div>
</form>
</div>
</div>
</div>
