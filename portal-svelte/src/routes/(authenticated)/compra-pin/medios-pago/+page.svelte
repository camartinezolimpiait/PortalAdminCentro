<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
let selectedMedio: string | null = $state(null);
const mediosPago = [
{ id: 'PSE', nombre: 'PSE - Débito bancario', icono: '🏦' },
{ id: 'TC', nombre: 'Tarjeta de Crédito', icono: '💳' },
{ id: 'NEQUI', nombre: 'Nequi', icono: '📱' },
{ id: 'EFECTIVO', nombre: 'Efectivo', icono: '💵' }
];
</script>
<svelte:head><title>Medios de Pago - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Medios de Pago</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={6} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
{#each mediosPago as medio}
<button
class="btn btn-outline btn-lg justify-start gap-3 {selectedMedio === medio.id ? 'btn-primary' : ''}"
onclick={() => { selectedMedio = medio.id; compraPinStore.setMedioPago(medio.id); }}
>
<span class="text-2xl">{medio.icono}</span>
{medio.nombre}
</button>
{/each}
</div>
<div class="card-actions justify-between mt-6">
<a href="/compra-pin/resumen" class="btn btn-ghost">← Anterior</a>
<a href="/compra-pin/confirmacion" class="btn btn-primary" class:btn-disabled={!selectedMedio}>Confirmar Pago →</a>
</div>
</div>
</div>
</div>
