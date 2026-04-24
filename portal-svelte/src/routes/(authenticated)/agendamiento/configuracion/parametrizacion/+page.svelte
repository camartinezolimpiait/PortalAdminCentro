<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>parametrizacion - Configuración</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">parametrizacion</h2>
<a href="/agendamiento/configuracion" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<p class="mb-4">Configuración de parametrizacion del agendamiento.</p>
<div class="alert alert-info">
<span>Los parámetros de configuración se cargan desde el servicio de agendamiento del backend.</span>
</div>
<div class="card-actions justify-end mt-4">
<button type="submit" class="btn btn-primary">Guardar</button>
</div>
</form>
</div>
</div>
</div>
