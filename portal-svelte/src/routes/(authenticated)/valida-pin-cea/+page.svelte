<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>Validar PIN CEA</title></svelte:head>
<div class="max-w-lg mx-auto space-y-4">
<h1 class="text-2xl font-bold">Validar PIN - CEA</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="form-control mb-4">
<label class="label" for="pin-cea"><span class="label-text">Número de PIN</span></label>
<input id="pin-cea" name="pin" type="text" class="input input-bordered" placeholder="Ingrese el PIN" required />
</div>
<button type="submit" class="btn btn-primary w-full">Validar</button>
</form>
</div>
</div>
</div>
