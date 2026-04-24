<script lang="ts">
import { enhance } from '$app/forms';
import StepIndicator from '$lib/components/shared/StepIndicator.svelte';
import { compraPinStore } from '$lib/stores/compra-pin';
</script>
<svelte:head><title>Datos Personales - Compra PIN</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra de PIN - Datos Personales</h1>
<StepIndicator steps={$compraPinStore.pasos} currentStep={2} />
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
<div class="form-control">
<label class="label" for="fechaNacimiento"><span class="label-text">Fecha de Nacimiento</span></label>
<input id="fechaNacimiento" name="fechaNacimiento" type="date" class="input input-bordered" />
</div>
<div class="form-control">
<label class="label" for="sexo"><span class="label-text">Sexo</span></label>
<select id="sexo" name="sexo" class="select select-bordered">
<option value="" disabled selected>Seleccione</option>
<option value="M">Masculino</option>
<option value="F">Femenino</option>
</select>
</div>
<div class="form-control">
<label class="label" for="correo"><span class="label-text">Correo Electrónico</span></label>
<input id="correo" name="correoElectronico" type="email" class="input input-bordered" />
</div>
<div class="form-control">
<label class="label" for="celular"><span class="label-text">Celular</span></label>
<input id="celular" name="celular" type="tel" class="input input-bordered" />
</div>
<div class="form-control md:col-span-2">
<label class="label" for="direccion"><span class="label-text">Dirección</span></label>
<input id="direccion" name="direccion" type="text" class="input input-bordered" />
</div>
</div>
<div class="card-actions justify-between mt-6">
<a href="/compra-pin/datos-basicos" class="btn btn-ghost">← Anterior</a>
<a href="/compra-pin/categorias" class="btn btn-primary">Siguiente →</a>
</div>
</form>
</div>
</div>
</div>
