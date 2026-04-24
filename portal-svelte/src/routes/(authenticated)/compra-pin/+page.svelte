<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
</script>
<svelte:head><title>Compra de PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={$compraPinStore.pasoActual} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<p>Para iniciar el proceso de compra de PIN, complete los datos básicos del solicitante.</p>
<div class="card-actions justify-end mt-4">
<a href="/compra-pin/datos-basicos" class="btn btn-primary">Iniciar Compra</a>
</div>
</div>
</div>
</div>
