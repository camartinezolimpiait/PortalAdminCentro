<script lang="ts">
import { page } from '$app/stores';
</script>

<div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
<div class="card bg-base-100 shadow-xl max-w-md w-full">
<div class="card-body text-center">
<div class="text-6xl mb-4">
{#if $page.status === 404}
🔍
{:else if $page.status === 403}
🚫
{:else}
⚠️
{/if}
</div>
<h1 class="card-title justify-center text-2xl">
Error {$page.status}
</h1>
<p class="text-base-content/70 mt-2">
{$page.error?.message || 'Ha ocurrido un error inesperado'}
</p>
<div class="card-actions justify-center mt-6">
<a href="/" class="btn btn-primary">Ir al inicio</a>
</div>
</div>
</div>
</div>
