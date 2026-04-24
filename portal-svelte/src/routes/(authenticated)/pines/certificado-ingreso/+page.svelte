<script lang="ts">
let { data } = $props();
</script>
<svelte:head><title>Certificado de Ingreso</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h2 class="text-xl font-bold">Certificado de Ingreso</h2>
<a href="/pines" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div class="card bg-base-100 shadow">
<div class="card-body">
<p>Funcionalidad de generación de certificados de ingreso de pines.</p>
<div class="alert alert-info mt-4">
<span>Esta funcionalidad requiere integración con el servicio de generación de certificados del backend.</span>
</div>
</div>
</div>
</div>
