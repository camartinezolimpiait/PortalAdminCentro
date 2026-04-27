<script lang="ts">
  import type { Snippet } from 'svelte';
  import { page } from '$app/stores';

  let { children }: { children: Snippet } = $props();

  const sections = [
    { href: '/configuracion/parametrizacion', label: 'Parametrización' },
    { href: '/configuracion/horario-atencion', label: 'Horario Atención' },
    { href: '/configuracion/horario-agendamiento', label: 'Horario Agendamiento' },
    { href: '/configuracion/perfil-milicencia', label: 'Perfil MiLicencia' },
    { href: '/configuracion/politica-agendamiento', label: 'Política Agendamiento' },
    { href: '/configuracion/configuracion-cupos', label: 'Configuración Cupos' },
    { href: '/configuracion/configuracion-reglas', label: 'Configuración Reglas' },
    { href: '/configuracion/data-contact', label: 'Datos Contacto' },
    { href: '/configuracion/facturacion', label: 'Facturación Electrónica' }
  ];
</script>

<div class="flex flex-col lg:flex-row gap-4">
  <aside class="w-full lg:w-64 shrink-0">
    <ul class="menu bg-base-200 rounded-box w-full">
      {#each sections as section}
        <li>
          <a href={section.href} class:active={$page.url.pathname.startsWith(section.href)}>
            {section.label}
          </a>
        </li>
      {/each}
    </ul>
  </aside>
  <div class="flex-1 min-w-0">
    {@render children()}
  </div>
</div>
