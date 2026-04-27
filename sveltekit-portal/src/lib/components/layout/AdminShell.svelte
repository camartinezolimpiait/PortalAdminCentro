<script lang="ts">
  import type { Snippet } from 'svelte';
  import type { UserSession } from '$lib/types/auth';
  import NavPrimary from '$lib/components/navigation/NavPrimary.svelte';
  import NavSidebar from '$lib/components/navigation/NavSidebar.svelte';

  interface Props {
    user: UserSession;
    children: Snippet;
  }

  let { user, children }: Props = $props();
  let drawerOpen = $state(false);

  const platformName = $derived(
    user.plataforma === 'CRC' ? 'MiCRC' :
    user.plataforma === 'CEA' ? 'MiCEA' :
    user.plataforma === 'Armas' ? 'MiArmas' : 'MiCDA'
  );
</script>

<div class="drawer lg:drawer-open">
  <input id="main-drawer" type="checkbox" class="drawer-toggle" bind:checked={drawerOpen} />

  <div class="drawer-content flex flex-col">
    <!-- Navbar -->
    <nav class="navbar bg-base-100 shadow-sm border-b" aria-label="Navegación principal">
      <div class="flex-none lg:hidden">
        <label for="main-drawer" class="btn btn-square btn-ghost" aria-label="Abrir menú">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-6 h-6 stroke-current">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </label>
      </div>

      <div class="flex-1">
        <span class="text-xl font-bold px-2">{platformName}</span>
      </div>

      <div class="flex-none hidden md:block">
        <NavPrimary items={user.menuListPrincipal ?? []} />
      </div>

      <!-- User dropdown -->
      <div class="dropdown dropdown-end">
        <div tabindex="0" role="button" class="btn btn-ghost gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5" aria-hidden="true">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z" />
          </svg>
          <span class="hidden sm:inline">{user.firstName || user.userName}</span>
        </div>
        <!-- svelte-ignore a11y_no_noninteractive_tabindex -->
        <ul tabindex="0" class="menu dropdown-content bg-base-200 rounded-box z-10 mt-3 w-52 p-2 shadow">
          <li>
            <form method="POST" action="/api/auth/logout">
              <button type="submit" class="w-full text-left">Cerrar sesión</button>
            </form>
          </li>
        </ul>
      </div>
    </nav>

    <!-- Page content -->
    <div class="p-4 lg:p-6">
      {@render children()}
    </div>
  </div>

  <!-- Sidebar -->
  <div class="drawer-side z-40">
    <label for="main-drawer" aria-label="Cerrar menú" class="drawer-overlay"></label>
    <aside class="bg-base-200 min-h-full w-64 p-4" aria-label="Menú lateral">
      <div class="mb-6 px-2">
        <span class="text-lg font-bold">{platformName}</span>
      </div>
      <NavSidebar items={user.menuList ?? []} onNavigate={() => (drawerOpen = false)} />
    </aside>
  </div>
</div>
