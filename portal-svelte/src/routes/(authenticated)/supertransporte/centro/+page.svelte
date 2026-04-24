<script lang="ts">
import { page } from '$app/stores';
let { data } = $props();
const sections = [
{ label: 'Info Básica', id: 'info-basica' },
{ label: 'Resolución', id: 'resolucion' },
{ label: 'Representante Legal', id: 'representante' },
{ label: 'Constitución', id: 'constitucion' },
{ label: 'Infraestructura', id: 'infraestructura' },
{ label: 'Profesionales Salud', id: 'profesionales-salud' },
{ label: 'Profesionales Certificadores', id: 'profesionales-cert' },
{ label: 'Póliza', id: 'poliza' },
{ label: 'Acreditación ONAC', id: 'acreditacion' },
{ label: 'Interconexión RUNT', id: 'runt' },
{ label: 'Registro REPS', id: 'reps' },
{ label: 'Homologado', id: 'homologado' }
];
</script>
<svelte:head><title>Centro CRC - SuperTransporte</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">Centro de Reconocimiento de Conductores</h1>
<a href="/supertransporte" class="btn btn-ghost btn-sm">← Volver</a>
</div>
{#if data.error}<div class="alert alert-error">{data.error}</div>{/if}
<div class="grid grid-cols-1 md:grid-cols-4 gap-4">
<aside class="md:col-span-1">
<ul class="menu bg-base-200 rounded-box w-full">
{#each sections as section}
<li><a href="#{section.id}" class="text-sm">{section.label}</a></li>
{/each}
</ul>
</aside>
<div class="md:col-span-3 space-y-6">
{#each sections as section}
<div id={section.id} class="card bg-base-100 shadow">
<div class="card-body">
<h2 class="card-title">{section.label}</h2>
<div class="alert alert-info">
<span>Datos de {section.label} se obtienen del servicio SICOV de SuperTransporte.</span>
</div>
</div>
</div>
{/each}
</div>
</div>
</div>
