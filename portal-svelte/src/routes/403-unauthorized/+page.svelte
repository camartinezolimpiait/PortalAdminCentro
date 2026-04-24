<!--
  403 Unauthorized Page
  Shown when user has valid session but insufficient permissions
-->
<svelte:head>
	<title>Acceso No Autorizado</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
	<div class="card bg-base-100 shadow-xl max-w-md w-full">
		<div class="card-body text-center">
			<div class="text-6xl mb-4">🚫</div>
			<h1 class="card-title justify-center text-2xl text-error">Acceso No Autorizado</h1>
			<p class="text-base-content/70 mt-2">
				No tiene los permisos necesarios para acceder a esta página.
				Contacte al administrador si considera que esto es un error.
			</p>
			<div class="card-actions justify-center mt-6">
				<a href="/pines" class="btn btn-primary">Ir al inicio</a>
				<a href="/login" class="btn btn-outline">Cerrar sesión</a>
			</div>
		</div>
	</div>
</div>
