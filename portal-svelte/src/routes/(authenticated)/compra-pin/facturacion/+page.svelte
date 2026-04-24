<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>Datos de Facturación</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Datos de Facturación Electrónica</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
<div class="form-control">
<label class="label" for="tipoDoc"><span class="label-text">Tipo Documento</span></label>
<select id="tipoDoc" name="tipoDocumento" class="select select-bordered">
<option value="NIT">NIT</option>
<option value="CC">Cédula de Ciudadanía</option>
</select>
</div>
<div class="form-control">
<label class="label" for="numDoc"><span class="label-text">Número Documento</span></label>
<input id="numDoc" name="numeroDocumento" type="text" class="input input-bordered" />
</div>
<div class="form-control md:col-span-2">
<label class="label" for="razonSocial"><span class="label-text">Razón Social</span></label>
<input id="razonSocial" name="razonSocial" type="text" class="input input-bordered" />
</div>
<div class="form-control md:col-span-2">
<label class="label" for="email"><span class="label-text">Correo Electrónico</span></label>
<input id="email" name="correoElectronico" type="email" class="input input-bordered" />
</div>
</div>
<div class="card-actions justify-end mt-6">
<button type="submit" class="btn btn-primary">Guardar</button>
</div>
</form>
</div>
</div>
</div>
