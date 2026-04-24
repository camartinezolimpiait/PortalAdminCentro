<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
</script>
<svelte:head><title>Confirmación - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Confirmación</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={7} />
<div class="card bg-base-100 shadow">
<div class="card-body text-center">
<div class="text-6xl mb-4">✅</div>
<h2 class="text-2xl font-bold text-success">¡Compra exitosa!</h2>
<p class="mt-2">Su PIN ha sido generado correctamente.</p>
{#if $compraPinStore.referenciaPago}
<p class="mt-2">Referencia de pago: <strong>{$compraPinStore.referenciaPago}</strong></p>
{/if}
<div class="card-actions justify-center mt-6">
<a href="/pines/activos" class="btn btn-primary">Ver Pines Activos</a>
<button class="btn btn-outline" onclick={() => compraPinStore.reset()}>
<a href="/compra-pin">Nueva Compra</a>
</button>
</div>
</div>
</div>
</div>
