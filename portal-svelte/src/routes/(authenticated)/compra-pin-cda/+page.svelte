<script lang="ts">
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
const pasos = [
{ paso: 1, nombre: 'Datos Básicos', ruta: '/compra-pin-cda/datos-basicos', completado: false },
{ paso: 2, nombre: 'Datos Personales', ruta: '/compra-pin-cda/datos-personales', completado: false },
{ paso: 3, nombre: 'Categorías', ruta: '/compra-pin-cda/categorias', completado: false },
{ paso: 4, nombre: 'Trámite', ruta: '/compra-pin-cda/tramite', completado: false },
{ paso: 5, nombre: 'Resumen', ruta: '/compra-pin-cda/resumen', completado: false },
{ paso: 6, nombre: 'Confirmación', ruta: '/compra-pin-cda/confirmacion', completado: false }
];
</script>
<svelte:head><title>Compra de PIN CDA</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - CDA</h1>
<StepIndicator steps={pasos} currentStep={1} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<p>Proceso de compra de PIN para Centros de Diagnóstico Automotriz.</p>
<div class="card-actions justify-end mt-4">
<a href="/compra-pin-cda/datos-basicos" class="btn btn-primary">Iniciar Compra</a>
</div>
</div>
</div>
</div>
