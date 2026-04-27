<script lang="ts">
  import { goto } from '$app/navigation';
  import { page } from '$app/stores';

  const user = $derived($page.data.user);
  $effect(() => {
    if (user) goto('/pines');
  });
</script>

<div class="hero min-h-screen bg-base-200">
  <div class="hero-content text-center">
    <div class="max-w-md">
      <h1 class="text-4xl font-bold">Portal Administrativo SISEC</h1>
      <p class="py-6">Sistema Integral de Seguridad en Centros</p>
      <a href="/login" class="btn btn-primary">Iniciar Sesión</a>
    </div>
  </div>
</div>
