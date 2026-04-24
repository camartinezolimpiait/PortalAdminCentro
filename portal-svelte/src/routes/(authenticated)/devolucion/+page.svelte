<script lang="ts">
import { enhance } from '$app/forms';
</script>
<svelte:head><title>Devolución de PIN</title></svelte:head>
<div class="max-w-lg mx-auto space-y-4">
<h1 class="text-2xl font-bold">Solicitar Devolución</h1>
<div class="card bg-base-100 shadow">
<div class="card-body">
<form method="POST" use:enhance>
<div class="form-control mb-3">
<label class="label"><span class="label-text">Número de PIN</span></label>
<input name="pin" type="text" class="input input-bordered" required />
</div>
<div class="form-control mb-4">
<label class="label"><span class="label-text">Motivo</span></label>
<textarea name="motivo" class="textarea textarea-bordered" rows="3" required></textarea>
</div>
<button type="submit" class="btn btn-primary w-full">Solicitar Devolución</button>
</form>
</div>
</div>
</div>
