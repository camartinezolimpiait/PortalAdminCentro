<!--
  Login Page
  Replaces: Pages/Login.razor + Shared/LoginLayout.razor
  SvelteKit Form Actions with DaisyUI components
-->
<script lang="ts">
	import { enhance } from '$app/forms';
	import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
	import BtnLoader from '$lib/components/shared/BtnLoader.svelte';

	let { data, form } = $props();

	let recoverMode = $derived(data.recoverMode || form?.recover === true);
	let showLoader = $state(false);
	let showModal = $state(false);
	let recoveredEmail = $state('');

	// Check if recovery was successful
	$effect(() => {
		if (form?.success && form?.recover) {
			showModal = true;
			recoveredEmail = form.email || '';
		}
	});
</script>

<svelte:head>
	<title>Portal Administrativo - Iniciar Sesión</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
	<div class="flex flex-col w-full max-w-sm">
		<h1 class="text-3xl font-bold text-center mb-6 text-primary">
			Portal <br /> Administrativo
		</h1>

		{#if !recoverMode}
			<!-- Login Form -->
			<div class="card bg-base-100 shadow-xl">
				<div class="card-body">
					<form
						method="POST"
						action="?/login"
						use:enhance={() => {
							showLoader = true;
							return async ({ update }) => {
								showLoader = false;
								await update();
							};
						}}
					>
						<div class="form-control mb-3">
							<label class="label" for="usuario">
								<span class="label-text">Nombre de usuario</span>
							</label>
							<input
								type="text"
								id="usuario"
								name="userName"
								class="input input-bordered w-full"
								value={form?.userName ?? ''}
								required
							/>
						</div>

						<div class="form-control mb-3">
							<label class="label" for="password">
								<span class="label-text">Contraseña</span>
							</label>
							<input
								type="password"
								id="password"
								name="passWord"
								class="input input-bordered w-full"
								required
							/>
						</div>

						<div class="form-control mb-4">
							<label class="label" for="tipocentro">
								<span class="label-text">Tipo de negocio</span>
							</label>
							<select
								name="plataforma"
								id="tipocentro"
								class="select select-bordered w-full"
								required
							>
								<option value="" disabled selected>Seleccione</option>
								{#each data.clientes as cliente}
									<option
										value={cliente.nombre}
										selected={form?.plataforma === cliente.nombre}
									>
										{cliente.nombre}
									</option>
								{/each}
							</select>
						</div>

						{#if !showLoader}
							<button class="btn btn-primary w-full" type="submit">
								Iniciar sesión
							</button>
						{/if}

						<BtnLoader {showLoader} />

						{#if form?.error && !form?.recover}
							<AlertMessage message={form.error} type="error" />
						{/if}

						<p class="text-xs text-base-content/60 mt-4 text-center">
							Este sitio está protegido por reCAPTCHA y se aplican la
							<a href="https://policies.google.com/privacy" class="link link-primary" target="_blank" rel="noopener noreferrer">
								política de privacidad
							</a>
							y
							<a href="https://policies.google.com/terms" class="link link-primary" target="_blank" rel="noopener noreferrer">
								términos del servicio
							</a>
							de Google.
						</p>
					</form>

					<div class="divider">O</div>

					<button
						class="btn btn-outline btn-sm"
						onclick={() => (recoverMode = true)}
					>
						¿Olvidó su contraseña?
					</button>
				</div>
			</div>
		{:else}
			<!-- Password Recovery Form -->
			<div class="card bg-base-100 shadow-xl">
				<div class="card-body">
					<form
						method="POST"
						action="?/recoverPassword"
						use:enhance={() => {
							showLoader = true;
							return async ({ update }) => {
								showLoader = false;
								await update();
							};
						}}
					>
						<div class="form-control mb-3">
							<label class="label" for="recover-usuario">
								<span class="label-text">Nombre de usuario</span>
							</label>
							<input
								type="text"
								id="recover-usuario"
								name="userName"
								class="input input-bordered w-full"
								value={form?.userName ?? ''}
								required
							/>
						</div>

						<div class="form-control mb-4">
							<label class="label" for="recover-tipo">
								<span class="label-text">Tipo de negocio</span>
							</label>
							<select
								name="plataforma"
								id="recover-tipo"
								class="select select-bordered w-full"
								required
							>
								<option value="" disabled selected>Seleccione</option>
								{#each data.clientes as cliente}
									<option value={cliente.nombre}>{cliente.nombre}</option>
								{/each}
							</select>
						</div>

						{#if !showLoader}
							<button class="btn btn-primary w-full" type="submit">
								Recuperar Contraseña
							</button>
						{/if}

						<BtnLoader {showLoader} />

						{#if form?.error && form?.recover}
							<AlertMessage message={form.error} type="error" />
						{/if}
					</form>

					<div class="text-center mt-4">
						<button
							class="btn btn-link btn-sm"
							onclick={() => (recoverMode = false)}
						>
							Iniciar Sesión
						</button>
					</div>
				</div>
			</div>
		{/if}
	</div>
</div>

<!-- Password Recovery Success Modal -->
{#if showModal}
	<dialog class="modal modal-open">
		<div class="modal-box">
			<h3 class="text-lg font-bold">Restablecer contraseña</h3>
			<p class="py-4">
				Hemos enviado una contraseña temporal al correo electrónico: {recoveredEmail}
			</p>
			<div class="modal-action">
				<a href="/login" class="btn btn-primary">Siguiente</a>
			</div>
		</div>
	</dialog>
{/if}
