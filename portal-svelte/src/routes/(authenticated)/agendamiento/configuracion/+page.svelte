<script lang="ts">
import { page } from '$app/stores';
const tabs = [
{ label: 'Cupos', href: '/agendamiento/configuracion/cupos' },
{ label: 'Reglas', href: '/agendamiento/configuracion/reglas' },
{ label: 'Horario', href: '/agendamiento/configuracion/horario' },
{ label: 'Parametrización', href: '/agendamiento/configuracion/parametrizacion' },
{ label: 'Perfil', href: '/agendamiento/configuracion/perfil' },
{ label: 'Política', href: '/agendamiento/configuracion/politica' },
{ label: 'Inicio', href: '/agendamiento/configuracion/inicio' }
];
</script>
<svelte:head><title>Configuración de Agendamiento</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">⚙️ Configuración de Agendamiento</h1>
<a href="/agendamiento" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div role="tablist" class="tabs tabs-boxed">
{#each tabs as tab}
<a role="tab" href={tab.href} class="tab {$page.url.pathname === tab.href ? 'tab-active' : ''}">{tab.label}</a>
{/each}
</div>
<div class="card bg-base-100 shadow">
<div class="card-body">
<p>Seleccione una opción de configuración del menú superior.</p>
</div>
</div>
</div>
