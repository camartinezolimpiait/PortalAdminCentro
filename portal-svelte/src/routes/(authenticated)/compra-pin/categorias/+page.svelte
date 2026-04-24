<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
let { data } = $props();
let selectedCategoria: number | null = $state(null);
</script>
<svelte:head><title>Categorías - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Categorías</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={3} />
<div class="card bg-base-100 shadow">
<div class="card-body">
{#if data.categorias.length === 0}
<div class="alert alert-warning">No hay categorías disponibles para este centro.</div>
{:else}
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
{#each data.categorias as cat}
<button
class="card bg-base-200 hover:bg-primary/10 cursor-pointer transition-colors {selectedCategoria === cat.id ? 'ring-2 ring-primary' : ''}"
onclick={() => { selectedCategoria = cat.id; compraPinStore.setCategoria(cat.id); }}
>
<div class="card-body p-4">
<h3 class="card-title text-sm">{cat.nombre}</h3>
{#if cat.descripcion}<p class="text-xs text-base-content/60">{cat.descripcion}</p>{/if}
</div>
</button>
{/each}
</div>
{/if}
<div class="card-actions justify-between mt-6">
<a href="/compra-pin/datos-personales" class="btn btn-ghost">← Anterior</a>
<a href="/compra-pin/tramite" class="btn btn-primary" class:btn-disabled={!selectedCategoria}>Siguiente →</a>
</div>
</div>
</div>
</div>
