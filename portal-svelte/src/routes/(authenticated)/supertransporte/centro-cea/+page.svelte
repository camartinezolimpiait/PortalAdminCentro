<script lang="ts">
let { data } = $props();
const sections = [
{ label: 'Info Básica', id: 'info-basica' },
{ label: 'Resolución Habilitación', id: 'resolucion' },
{ label: 'Representante Legal', id: 'representante' },
{ label: 'Constitución', id: 'constitucion' },
{ label: 'Licencia Funcionamiento', id: 'licencia' },
{ label: 'Infraestructura', id: 'infraestructura' },
{ label: 'Instructores', id: 'instructores' },
{ label: 'Vehículos', id: 'vehiculos' },
{ label: 'Propietarios', id: 'propietarios' },
{ label: 'Póliza', id: 'poliza' },
{ label: 'Certificación OEC', id: 'certificacion' },
{ label: 'Programas/Convenios', id: 'programas' },
{ label: 'Homologado', id: 'homologado' }
];
</script>
<svelte:head><title>Centro CEA - SuperTransporte</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">Centro de Enseñanza Automovilística</h1>
<a href="/supertransporte" class="btn btn-ghost btn-sm">← Volver</a>
</div>
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
