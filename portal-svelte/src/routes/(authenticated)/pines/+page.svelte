<!--
  Pines Module - Main Page
  Replaces: Pages/Pines/Pines.razor
  Hub page with tabs for different PIN states
-->
<script lang="ts">
import { page } from '$app/stores';

let { data } = $props();

const tabs = [
{ label: 'Activos', href: '/pines/activos', icon: '✓' },
{ label: 'Usados', href: '/pines/usados', icon: '📋' },
{ label: 'Devoluciones', href: '/pines/devoluciones', icon: '↩' },
{ label: 'Dispersiones', href: '/pines/dispersiones', icon: '💰' },
{ label: 'Certificado', href: '/pines/certificado-ingreso', icon: '📄' },
{ label: 'Búsqueda', href: '/pines/busqueda', icon: '🔍' },
{ label: 'Asociados', href: '/pines/asociados', icon: '🔗' }
];
</script>

<svelte:head>
<title>Gestión de Pines</title>
</svelte:head>

<div class="space-y-4">
<h1 class="text-2xl font-bold">Gestión de Pines</h1>

<div role="tablist" class="tabs tabs-boxed">
{#each tabs as tab}
<a
role="tab"
href={tab.href}
class="tab {$page.url.pathname === tab.href ? 'tab-active' : ''}"
>
<span class="mr-1">{tab.icon}</span>
<span class="hidden sm:inline">{tab.label}</span>
</a>
{/each}
</div>

<div class="card bg-base-100 shadow">
<div class="card-body">
<p class="text-base-content/60">
Seleccione una opción del menú superior para ver los pines correspondientes.
</p>
</div>
</div>
</div>
