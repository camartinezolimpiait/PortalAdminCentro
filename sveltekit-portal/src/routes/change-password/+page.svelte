<script lang="ts">
  import BtnLoader from '$lib/components/ui/BtnLoader.svelte';
  import FormField from '$lib/components/forms/FormField.svelte';

  let loading = $state(false);
</script>

<div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
  <div class="card w-full max-w-md bg-base-100 shadow-xl">
    <div class="card-body">
      <h1 class="card-title text-2xl font-bold mb-4 justify-center">Cambiar Contraseña</h1>

      <form method="POST">
        <FormField label="Contraseña actual" name="currentPassword" required>
          <input type="password" name="currentPassword" id="currentPassword" class="input input-bordered w-full" required />
        </FormField>

        <FormField label="Nueva contraseña" name="newPassword" required>
          <input type="password" name="newPassword" id="newPassword" class="input input-bordered w-full" required />
        </FormField>

        <FormField label="Confirmar contraseña" name="confirmPassword" required>
          <input type="password" name="confirmPassword" id="confirmPassword" class="input input-bordered w-full" required />
        </FormField>

        <div class="mt-6">
          <BtnLoader {loading} text="Cambiar contraseña" type="submit" variant="btn-primary w-full" />
        </div>

        <div class="text-center mt-4">
          <a href="/login" class="link link-primary text-sm">Volver a iniciar sesión</a>
        </div>
      </form>
    </div>
  </div>
</div>
