<script lang="ts">
  import type { Snippet } from 'svelte';
  import { page } from '$app/stores';

  let { children }: { children: Snippet } = $props();

  const sections = [
    { href: '/supertransporte/centro-cea/informacion-basica', label: 'Información Básica' },
    { href: '/supertransporte/centro-cea/informacion-constitucion', label: 'Constitución' },
    { href: '/supertransporte/centro-cea/informacion-vehiculos', label: 'Vehículos' },
    { href: '/supertransporte/centro-cea/infraestructura', label: 'Infraestructura' },
    { href: '/supertransporte/centro-cea/informacion-propietarios', label: 'Propietarios' },
    { href: '/supertransporte/centro-cea/instructor', label: 'Instructores' },
    { href: '/supertransporte/centro-cea/certificacion-oec', label: 'Certificación OEC' },
    { href: '/supertransporte/centro-cea/informacion-homologado', label: 'Homologado' },
    { href: '/supertransporte/centro-cea/programas-convenio', label: 'Programas y Convenios' },
    { href: '/supertransporte/centro-cea/poliza', label: 'Póliza' },
    { href: '/supertransporte/centro-cea/resolucion-habilitacion', label: 'Resolución Habilitación' },
    { href: '/supertransporte/centro-cea/licencia-funcionamiento', label: 'Licencia Funcionamiento' },
    { href: '/supertransporte/centro-cea/representante-legal', label: 'Representante Legal' }
  ];
</script>

<h1 class="text-2xl font-bold mb-4">SuperTransporte - Centro CEA</h1>
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
