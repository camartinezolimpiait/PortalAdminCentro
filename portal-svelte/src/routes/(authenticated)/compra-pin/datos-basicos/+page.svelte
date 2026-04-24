<script lang="ts">
import { enhance } from '$app/forms';
import AlertMessage from '$lib/components/shared/AlertMessage.svelte';
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
let { data, form } = $props();
</script>
<svelte:head><title>Datos Básicos - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Datos Básicos</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={1} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
<div class="form-control">
<label class="label" for="tipoDocumento"><span class="label-text">Tipo de Documento *</span></label>
<select id="tipoDocumento" name="tipoDocumento" class="select select-bordered" required>
<option value="" disabled selected>Seleccione</option>
{#each data.tiposDocumento as tipo}
<option value={tipo.nombre}>{tipo.nombre}</option>
{/each}
</select>
</div>
<div class="form-control">
<label class="label" for="numeroDocumento"><span class="label-text">Número de Documento *</span></label>
<input id="numeroDocumento" name="numeroDocumento" type="text" class="input input-bordered" required value={form?.numeroDocumento ?? ''} />
</div>
<div class="form-control">
<label class="label" for="primerNombre"><span class="label-text">Primer Nombre *</span></label>
<input id="primerNombre" name="primerNombre" type="text" class="input input-bordered" required value={form?.primerNombre ?? ''} />
</div>
<div class="form-control">
<label class="label" for="segundoNombre"><span class="label-text">Segundo Nombre</span></label>
<input id="segundoNombre" name="segundoNombre" type="text" class="input input-bordered" />
</div>
<div class="form-control">
<label class="label" for="primerApellido"><span class="label-text">Primer Apellido *</span></label>
<input id="primerApellido" name="primerApellido" type="text" class="input input-bordered" required value={form?.primerApellido ?? ''} />
</div>
<div class="form-control">
<label class="label" for="segundoApellido"><span class="label-text">Segundo Apellido</span></label>
<input id="segundoApellido" name="segundoApellido" type="text" class="input input-bordered" />
</div>
</div>
{#if form?.error}<AlertMessage message={form.error} type="error" />{/if}
<div class="card-actions justify-end mt-6">
<a href="/compra-pin" class="btn btn-ghost">Cancelar</a>
<button type="submit" class="btn btn-primary">Siguiente →</button>
</div>
</form>
</div>
</div>
</div>
