<script lang="ts">
let { data } = $props();
const plataforma = $derived(data.user?.plataforma || '');
</script>
<svelte:head><title>SuperTransporte</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">SuperTransporte - Información del Centro</h1>
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
{#if plataforma === 'CRC' || plataforma === ''}
<a href="/supertransporte/centro" class="card bg-base-100 shadow hover:shadow-lg transition-shadow">
<div class="card-body"><h2 class="card-title">🏥 Centro CRC</h2><p>Información del Centro de Reconocimiento de Conductores</p></div>
</a>
{/if}
{#if plataforma === 'CEA' || plataforma === ''}
<a href="/supertransporte/centro-cea" class="card bg-base-100 shadow hover:shadow-lg transition-shadow">
<div class="card-body"><h2 class="card-title">🎓 Centro CEA</h2><p>Información del Centro de Enseñanza Automovilística</p></div>
</a>
{/if}
<a href="/supertransporte/pqrsf" class="card bg-base-100 shadow hover:shadow-lg transition-shadow">
<div class="card-body"><h2 class="card-title">📋 PQRSF</h2><p>Peticiones, quejas, reclamos, sugerencias y felicitaciones</p></div>
</a>
</div>
</div>
