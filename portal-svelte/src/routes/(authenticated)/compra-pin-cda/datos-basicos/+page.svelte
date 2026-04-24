<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>Datos Básicos - Compra PIN CDA</title></svelte:head>
<div class="space-y-4">
<h1 class="text-2xl font-bold">Compra PIN CDA - Datos Básicos</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
<div class="form-control">
<label class="label"><span class="label-text">Tipo Documento *</span></label>
<select name="tipoDocumento" class="select select-bordered" required>
<option value="" disabled selected>Seleccione</option>
<option value="CC">Cédula de Ciudadanía</option>
<option value="CE">Cédula de Extranjería</option>
<option value="PA">Pasaporte</option>
</select>
</div>
<div class="form-control">
<label class="label"><span class="label-text">Número Documento *</span></label>
<input name="numeroDocumento" type="text" class="input input-bordered" required />
</div>
<div class="form-control">
<label class="label"><span class="label-text">Primer Nombre *</span></label>
<input name="primerNombre" type="text" class="input input-bordered" required />
</div>
<div class="form-control">
<label class="label"><span class="label-text">Primer Apellido *</span></label>
<input name="primerApellido" type="text" class="input input-bordered" required />
</div>
</div>
<div class="card-actions justify-between mt-6">
<a href="/compra-pin-cda" class="btn btn-ghost">Cancelar</a>
<a href="/compra-pin-cda/datos-personales" class="btn btn-primary">Siguiente →</a>
</div>
</form>
</div>
</div>
</div>
