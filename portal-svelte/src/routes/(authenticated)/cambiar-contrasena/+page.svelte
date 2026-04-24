<script lang="ts">
import { enhance } from '$app/forms';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
let { form } = $props();
</script>
<svelte:head><title>Cambiar Contraseña</title></svelte:head>
<div class="max-w-md mx-auto space-y-4">
<h1 class="text-2xl font-bold">Cambiar Contraseña</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
{#if form?.success}
<AlertMessage message="Contraseña cambiada exitosamente" type="success" />
{/if}
<form method="POST" use:enhance>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Contraseña actual</span></label>
<input name="currentPassword" type="password" class="input input-bordered" required />
</div>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Nueva contraseña</span></label>
<input name="newPassword" type="password" class="input input-bordered" required minlength="8" />
</div>
<div class="form-control mb-4">
<label class="label"><span class="label-text">Confirmar nueva contraseña</span></label>
<input name="confirmPassword" type="password" class="input input-bordered" required minlength="8" />
</div>
{#if form?.error}<AlertMessage message={form.error} type="error" />{/if}
<button type="submit" class="btn btn-primary w-full">Cambiar Contraseña</button>
</form>
</div>
</div>
</div>
