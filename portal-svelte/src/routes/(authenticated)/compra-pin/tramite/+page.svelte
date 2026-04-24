<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
let selectedTramite: number | null = $state(null);
const tramites = [
{ id: 1, nombre: 'Primera vez' },
{ id: 2, nombre: 'Recategorización' },
{ id: 3, nombre: 'Refrendación' },
{ id: 4, nombre: 'Duplicado' }
];
</script>
<svelte:head><title>Trámite - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Tipo de Trámite</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={4} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
{#each tramites as tramite}
<button
class="btn btn-outline btn-lg justify-start {selectedTramite === tramite.id ? 'btn-primary' : ''}"
onclick={() => { selectedTramite = tramite.id; compraPinStore.setTramite(tramite.id); }}
>
{tramite.nombre}
</button>
{/each}
</div>
<div class="card-actions justify-between mt-6">
<a href="/compra-pin/categorias" class="btn btn-ghost">← Anterior</a>
<a href="/compra-pin/resumen" class="btn btn-primary" class:btn-disabled={!selectedTramite}>Siguiente →</a>
</div>
</div>
</div>
</div>
