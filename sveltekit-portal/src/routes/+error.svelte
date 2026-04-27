<script lang="ts">
  import { page } from '$app/stores';
</script>

<div class="hero min-h-[60vh]">
  <div class="hero-content text-center">
    <div class="max-w-md">
      <h1 class="text-6xl font-bold text-error">{$page.status}</h1>
      <p class="py-6 text-lg">{$page.error?.message || 'Ha ocurrido un error inesperado'}</p>
      <a href="/" class="btn btn-primary">Volver al inicio</a>
    </div>
  </div>
</div>
