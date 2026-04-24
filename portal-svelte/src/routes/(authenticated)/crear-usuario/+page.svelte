<script lang="ts">
import { enhance } from '$app/forms';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
let { form } = $props();
</script>
<svelte:head><title>Crear Usuario</title></svelte:head>
<div class="max-w-lg mx-auto space-y-4">
<h1 class="text-2xl font-bold">Crear Usuario</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
{#if form?.success}<AlertMessage message="Usuario creado exitosamente" type="success" />{/if}
<form method="POST" use:enhance>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Nombre de usuario *</span></label>
<input name="userName" type="text" class="input input-bordered" required />
</div>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Correo electrónico *</span></label>
<input name="email" type="email" class="input input-bordered" required />
</div>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Nombres *</span></label>
<input name="firstName" type="text" class="input input-bordered" required />
</div>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Apellidos *</span></label>
<input name="lastName" type="text" class="input input-bordered" required />
</div>
<div class="form-control mb-4">
<label class="label"><span class="label-text">Perfil</span></label>
<select name="perfil" class="select select-bordered">
<option value="Director">Director</option>
<option value="Auditor">Auditor</option>
<option value="Investigador">Investigador</option>
<option value="Instructor">Instructor</option>
</select>
</div>
{#if form?.error}<AlertMessage message={form.error} type="error" />{/if}
<button type="submit" class="btn btn-primary w-full">Crear Usuario</button>
</form>
</div>
</div>
</div>
