<script lang="ts">
  import { enhance } from '$app/forms';
  import BtnLoader from '$lib/components/ui/BtnLoader.svelte';
  import AlertMessage from '$lib/components/ui/AlertMessage.svelte';
  import FormField from '$lib/components/forms/FormField.svelte';
  import Modal from '$lib/components/ui/Modal.svelte';

  interface Props {
    form: {
      error?: string;
      userName?: string;
      mustChangePassword?: boolean;
      tempPassword?: string;
    } | null;
  }

  let { form }: Props = $props();
  let loading = $state(false);
  let selectedPlatform = $state('CRC');

  const platforms = [
    { id: 'CRC', label: 'CRC - Centro de Reconocimiento' },
    { id: 'CEA', label: 'CEA - Centro de Enseñanza' },
    { id: 'CDA', label: 'CDA - Centro de Diagnóstico' },
    { id: 'Armas', label: 'Armas' }
  ];

  const showPasswordModal = $derived(form?.mustChangePassword ?? false);
</script>

<div class="min-h-screen flex items-center justify-center bg-base-200 p-4">
  <div class="card w-full max-w-md bg-base-100 shadow-xl">
    <div class="card-body">
      <h1 class="card-title text-2xl font-bold mb-4 justify-center">Portal SISEC</h1>
      <p class="text-center text-base-content/70 mb-6">Iniciar sesión</p>

      {#if form?.error}
        <AlertMessage type="error" message={form.error} dismissible />
      {/if}

      <form
        method="POST"
        use:enhance={() => {
          loading = true;
          return async ({ update }) => {
            loading = false;
            await update();
          };
        }}
      >
        <div class="mb-4">
          <span class="label-text font-medium mb-2 block">Plataforma</span>
          <div class="grid grid-cols-2 gap-2">
            {#each platforms as platform}
              <label class="label cursor-pointer justify-start gap-2 p-2 rounded-lg hover:bg-base-200">
                <input
                  type="radio"
                  name="plataforma"
                  value={platform.id}
                  class="radio radio-primary radio-sm"
                  checked={selectedPlatform === platform.id}
                  onchange={() => (selectedPlatform = platform.id)}
                />
                <span class="label-text text-sm">{platform.label}</span>
              </label>
            {/each}
          </div>
        </div>

        <FormField label="Usuario" name="userName" required>
          <input
            type="text"
            name="userName"
            id="userName"
            class="input input-bordered w-full"
            value={form?.userName ?? ''}
            required
            autocomplete="username"
          />
        </FormField>

        <FormField label="Contraseña" name="password" required>
          <input
            type="password"
            name="password"
            id="password"
            class="input input-bordered w-full"
            required
            autocomplete="current-password"
          />
        </FormField>

        <div class="mt-6">
          <BtnLoader {loading} text="Iniciar sesión" type="submit" variant="btn-primary w-full" />
        </div>

        <div class="text-center mt-4">
          <a href="/change-password" class="link link-primary text-sm">¿Olvidaste tu contraseña?</a>
        </div>
      </form>
    </div>
  </div>
</div>

<Modal open={showPasswordModal} title="Cambio de contraseña requerido" onclose={() => {}}>
  <p class="py-4">Debes cambiar tu contraseña temporal antes de continuar.</p>
  <div class="modal-action">
    <a href="/change-password" class="btn btn-primary">Cambiar contraseña</a>
  </div>
</Modal>
