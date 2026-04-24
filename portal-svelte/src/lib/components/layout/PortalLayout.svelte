<!--
  PortalLayout.svelte
  Replaces: Shared/PortalLayout.razor
  Main admin layout with navbar, user dropdown, and content area
  Uses DaisyUI navbar, dropdown, drawer components
-->
<script lang="ts">
	import NavMenuPrincipal from '$lib/components/navigation/NavMenuPrincipal.svelte';
	import NavMenuPortal from '$lib/components/navigation/NavMenuPortal.svelte';
	import type { SessionUser } from '$lib/types/models';
	import type { Snippet } from 'svelte';

	let {
		user,
		children
	}: {
		user: SessionUser | null;
		children: Snippet;
	} = $props();

	let drawerOpen = $state(false);

	function getLogo(plataforma: string | undefined): string {
		switch (plataforma) {
			case 'CRC':
				return '🏥 Mi CRC';
			case 'CEA':
				return '🎓 Mi CEA';
			case 'Armas':
				return '🔫 Mi Armas';
			default:
				return '🚗 Mi CDA';
		}
	}
</script>

<div class="drawer lg:drawer-open">
	<input id="portal-drawer" type="checkbox" class="drawer-toggle" bind:checked={drawerOpen} />

	<div class="drawer-content flex flex-col min-h-screen">
		<!-- Navbar -->
		<div class="navbar bg-base-100 shadow-sm sticky top-0 z-30">
			<!-- Mobile hamburger -->
			<div class="flex-none lg:hidden">
				<label for="portal-drawer" class="btn btn-square btn-ghost" aria-label="Abrir menú">
					<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
						class="inline-block h-5 w-5 stroke-current">
						<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
							d="M4 6h16M4 12h16M4 18h16" />
					</svg>
				</label>
			</div>

			<!-- Logo -->
			<div class="flex-none">
				<a href="/pines" class="btn btn-ghost text-xl normal-case">
					{getLogo(user?.plataforma)}
				</a>
			</div>

			<div class="divider divider-horizontal mx-0 hidden lg:flex"></div>

			<!-- Main nav (desktop) -->
			<div class="flex-1 hidden lg:flex">
				{#if user}
					<NavMenuPrincipal
						menuItems={user.menuPrincipal}
						plataforma={user.plataforma}
					/>
				{/if}
			</div>

			<!-- User dropdown -->
			<div class="flex-none">
				{#if user}
					<div class="dropdown dropdown-end">
						<div tabindex="0" role="button" class="btn btn-ghost gap-2">
							<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
								stroke-width="1.5" stroke="currentColor" class="w-5 h-5">
								<path stroke-linecap="round" stroke-linejoin="round"
									d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
							</svg>
							<span class="hidden sm:inline font-semibold">
								{user.firstName || user.userName}
							</span>
						</div>
						<!-- svelte-ignore a11y_no_noninteractive_tabindex -->
						<ul tabindex="0"
							class="menu dropdown-content bg-base-200 rounded-box z-50 mt-3 w-52 p-2 shadow-lg">
							<li>
								<a href="/cambiar-contrasena">
									<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
										stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
										<path stroke-linecap="round" stroke-linejoin="round"
											d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
									</svg>
									Cambiar contraseña
								</a>
							</li>
							<li>
								<form method="POST" action="/api/auth/logout">
									<button type="submit" class="w-full text-left flex items-center gap-2">
										<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
											stroke-width="1.5" stroke="currentColor" class="w-4 h-4">
											<path stroke-linecap="round" stroke-linejoin="round"
												d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
										</svg>
										Cerrar sesión
									</button>
								</form>
							</li>
						</ul>
					</div>
				{/if}
			</div>
		</div>

		<!-- Main content -->
		<main class="flex-1 p-4 md:p-6">
			{@render children()}
		</main>
	</div>

	<!-- Sidebar drawer (mobile) -->
	<div class="drawer-side z-40">
		<label for="portal-drawer" aria-label="Cerrar menú" class="drawer-overlay"></label>
		<aside class="bg-base-200 min-h-full w-64 p-4">
			<div class="mb-4">
				<span class="text-lg font-bold">{getLogo(user?.plataforma)}</span>
			</div>
			{#if user}
				<NavMenuPortal
					menuItems={user.menuItems}
					subMenuItems={user.subMenuItems}
				/>
			{/if}
		</aside>
	</div>
</div>
