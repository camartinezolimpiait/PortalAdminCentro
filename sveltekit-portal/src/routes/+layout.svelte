<script lang="ts">
  import '../app.css';
  import type { Snippet } from 'svelte';
  import AdminShell from '$lib/components/layout/AdminShell.svelte';
  import ToastContainer from '$lib/components/ui/ToastContainer.svelte';

  interface Props {
    data: { user: import('$lib/types/auth').UserSession | null };
    children: Snippet;
  }

  let { data, children }: Props = $props();
  const isAuthenticated = $derived(!!data.user);
</script>

<svelte:head>
  <title>Portal Administrativo SISEC</title>
</svelte:head>

<a href="#main-content" class="skip-link">Ir al contenido principal</a>

{#if isAuthenticated}
  <AdminShell user={data.user!}>
    <main id="main-content">
      {@render children()}
    </main>
  </AdminShell>
{:else}
  <main id="main-content">
    {@render children()}
  </main>
{/if}

<ToastContainer />
