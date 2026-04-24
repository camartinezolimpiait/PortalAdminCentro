<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
</script>
<svelte:head><title>Resumen - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Resumen</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={5} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<div class="overflow-x-auto">
<table class="table">
<tbody>
<tr><td class="font-semibold">Categoría</td><td>{$compraPinStore.categoriaId ?? 'No seleccionada'}</td></tr>
<tr><td class="font-semibold">Trámite</td><td>{$compraPinStore.tramiteId ?? 'No seleccionado'}</td></tr>
<tr><td class="font-semibold">Valor Total</td><td class="text-lg font-bold text-primary">${$compraPinStore.valorTotal.toLocaleString('es-CO')}</td></tr>
</tbody>
</table>
</div>
<div class="card-actions justify-between mt-6">
<a href="/compra-pin/tramite" class="btn btn-ghost">← Anterior</a>
<a href="/compra-pin/medios-pago" class="btn btn-primary">Continuar al Pago →</a>
</div>
</div>
</div>
</div>
