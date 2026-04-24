<script lang="ts">
import { page } from '$app/stores';
const tabs = [
{ label: 'Artículos', href: '/facturacion/configurar/articulos' },
{ label: 'Numeración', href: '/facturacion/configurar/numeracion' },
{ label: 'Credenciales', href: '/facturacion/configurar/credenciales' },
{ label: 'Comportamiento', href: '/facturacion/configurar/comportamiento' },
{ label: 'Emisión', href: '/facturacion/configurar/emision' },
{ label: 'Estado', href: '/facturacion/configurar/estado' }
];
</script>
<svelte:head><title>Configurar Facturación</title></svelte:head>
<div class="space-y-4">
<div class="flex items-center justify-between">
<h1 class="text-2xl font-bold">Configurar Facturación</h1>
<a href="/facturacion" class="btn btn-ghost btn-sm">← Volver</a>
</div>
<div role="tablist" class="tabs tabs-boxed">
{#each tabs as tab}
<a role="tab" href={tab.href} class="tab {$page.url.pathname === tab.href ? 'tab-active' : ''}">{tab.label}</a>
{/each}
</div>
<div class="card bg-base-100 shadow">
<div class="card-body"><p>Seleccione una opción de configuración.</p></div>
</div>
</div>
