<script lang="ts">
let { data } = $props();
</script>
<svelte:head><title>Blog</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Blog</h1>
{#if data.articles.length === 0}
<div class="alert alert-info">No hay artículos disponibles.</div>
{:else}
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
{#each data.articles as article}
<div class="card bg-base-100 shadow hover:shadow-lg transition-shadow">
<div class="card-body">
<h2 class="card-title">{article.title || article.attributes?.title || 'Sin título'}</h2>
<p class="text-sm text-base-content/60">{article.description || article.attributes?.description || ''}</p>
</div>
</div>
{/each}
</div>
{/if}
</div>
