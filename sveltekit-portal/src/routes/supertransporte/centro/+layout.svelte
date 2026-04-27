<script lang="ts">
  import type { Snippet } from 'svelte';
  import { page } from '$app/stores';

  let { children }: { children: Snippet } = $props();

  const sections = [
    { href: '/supertransporte/centro/informacion-basica', label: 'Información Básica' },
    { href: '/supertransporte/centro/informacion-constitucion', label: 'Constitución' },
    { href: '/supertransporte/centro/infraestructura', label: 'Infraestructura' },
    { href: '/supertransporte/centro/profesional-salud', label: 'Profesionales Salud' },
    { href: '/supertransporte/centro/profesional-certificadores', label: 'Certificadores' },
    { href: '/supertransporte/centro/acreditacion-onac', label: 'Acreditación ONAC' },
    { href: '/supertransporte/centro/informacion-homologado', label: 'Homologado' },
    { href: '/supertransporte/centro/registro-reps', label: 'Registro REPS' },
    { href: '/supertransporte/centro/interconexion-runt', label: 'Interconexión RUNT' },
    { href: '/supertransporte/centro/resolucion-habilitacion', label: 'Resolución Habilitación' },
    { href: '/supertransporte/centro/poliza', label: 'Póliza' },
    { href: '/supertransporte/centro/representante-legal', label: 'Representante Legal' }
  ];
</script>

<h1 class="text-2xl font-bold mb-4">SuperTransporte - Centro CRC</h1>
<div class="flex flex-col lg:flex-row gap-4">
  <aside class="w-full lg:w-64 shrink-0">
    <ul class="menu bg-base-200 rounded-box w-full">
      {#each sections as section}
        <li>
          <a href={section.href} class:active={$page.url.pathname === section.href}>
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
