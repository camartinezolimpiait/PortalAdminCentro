# Tematización y Accesibilidad con DaisyUI

## Sistema de Temas

### Mapeo de Colores: Original → DaisyUI Semantic Tokens

| Color Original (Blazor CSS) | Token DaisyUI | Uso |
|------------------------------|--------------|-----|
| `#1E3A5F` (azul oscuro) | `primary` | Botones principales, links, acciones primarias |
| `#2563EB` (azul medio) | `primary-focus` | Hover de botones principales |
| `#FFFFFF` (blanco) | `base-100` | Fondo principal |
| `#F3F4F6` (gris claro) | `base-200` | Fondo secundario (sidebar, cards) |
| `#E5E7EB` (gris borde) | `base-300` | Bordes, separadores |
| `#111827` (casi negro) | `base-content` | Texto principal |
| `#6B7280` (gris medio) | `neutral` | Texto secundario, iconos inactivos |
| `#10B981` (verde) | `success` | Confirmaciones, estados activos |
| `#EF4444` (rojo) | `error` | Errores, alertas críticas, eliminar |
| `#F59E0B` (amarillo) | `warning` | Advertencias, estados pendientes |
| `#3B82F6` (azul info) | `info` | Información, tooltips |
| `#8B5CF6` (morado) | `secondary` | Acciones secundarias |
| `#EC4899` (rosa) | `accent` | Destacados, badges especiales |

### Configuración de Tema en tailwind.config.js

```javascript
// tailwind.config.js
module.exports = {
  content: ['./src/**/*.{html,js,svelte,ts}'],
  theme: {
    extend: {}
  },
  plugins: [require('daisyui')],
  daisyui: {
    themes: [
      {
        sisec: {
          'primary': '#1E3A5F',
          'primary-content': '#FFFFFF',
          'secondary': '#8B5CF6',
          'secondary-content': '#FFFFFF',
          'accent': '#EC4899',
          'accent-content': '#FFFFFF',
          'neutral': '#6B7280',
          'neutral-content': '#FFFFFF',
          'base-100': '#FFFFFF',
          'base-200': '#F3F4F6',
          'base-300': '#E5E7EB',
          'base-content': '#111827',
          'info': '#3B82F6',
          'info-content': '#FFFFFF',
          'success': '#10B981',
          'success-content': '#FFFFFF',
          'warning': '#F59E0B',
          'warning-content': '#111827',
          'error': '#EF4444',
          'error-content': '#FFFFFF',
        }
      }
    ]
  }
};
```

---

## Mapeo de Componentes de Terceros → DaisyUI

### Blazored.Toast → DaisyUI Toast + Svelte Store

| Blazored.Toast | DaisyUI Equivalente | Implementación |
|----------------|--------------------|--------------:|
| `toastService.ShowSuccess("msg")` | `toast.success("msg")` | Store: `$lib/stores/toast.ts` |
| `toastService.ShowError("msg")` | `toast.error("msg")` | Componente: `ToastContainer.svelte` |
| `toastService.ShowWarning("msg")` | `toast.warning("msg")` | Clases: `alert alert-warning` |
| `toastService.ShowInfo("msg")` | `toast.info("msg")` | Clases: `alert alert-info` |

```svelte
<!-- ToastContainer.svelte -->
<div class="toast toast-end toast-top z-50">
  {#each $toasts as toast (toast.id)}
    <div class="alert alert-{toast.type} shadow-lg">
      <span>{toast.message}</span>
      <button class="btn btn-ghost btn-sm" on:click={() => dismiss(toast.id)}>✕</button>
    </div>
  {/each}
</div>
```

### BlazorPro.Spinkit → DaisyUI Loading

| Spinkit | DaisyUI | Clases |
|---------|---------|--------|
| `<SpinLoader>` | `<span class="loading">` | `loading loading-spinner loading-lg` |
| `<Chase>` | Loading dots | `loading loading-dots loading-lg` |
| `<Circle>` | Loading ring | `loading loading-ring loading-lg` |
| `<Bounce>` | Loading ball | `loading loading-ball loading-lg` |
| Overlay spinner | DaisyUI modal + loading | `modal modal-open` + `loading` |

```svelte
<!-- Reemplazo de SpinLoader como overlay -->
{#if isLoading}
  <div class="fixed inset-0 bg-base-300/50 flex items-center justify-center z-50">
    <span class="loading loading-spinner loading-lg text-primary"></span>
  </div>
{/if}
```

### QuickGrid / GridDinamico → DaisyUI Table

| QuickGrid | DaisyUI | Componente |
|-----------|---------|-----------|
| `<QuickGrid Items="@items">` | `<table class="table">` | `DynamicGrid.svelte` |
| `<PropertyColumn>` | `<th>` + `<td>` | Columnas dinámicas |
| `<TemplateColumn>` | Slot/snippet | Templates custom |
| Sorting | `on:click` en headers | Sort reactivo |
| Pagination | `join` (DaisyUI) | Paginador custom |

```svelte
<!-- DynamicGrid.svelte simplificado -->
<div class="overflow-x-auto">
  <table class="table table-zebra">
    <thead>
      <tr>
        {#each columns as col}
          <th class="cursor-pointer" on:click={() => sort(col.key)}>
            {col.label}
            {#if sortKey === col.key}
              <span>{sortDir === 'asc' ? '▲' : '▼'}</span>
            {/if}
          </th>
        {/each}
      </tr>
    </thead>
    <tbody>
      {#each paginatedData as row}
        <tr>
          {#each columns as col}
            <td>{row[col.key]}</td>
          {/each}
        </tr>
      {/each}
    </tbody>
  </table>
  <!-- Paginación -->
  <div class="join mt-4">
    {#each pages as p}
      <button class="join-item btn" class:btn-active={p === currentPage}
        on:click={() => currentPage = p}>{p}</button>
    {/each}
  </div>
</div>
```

### EditForm → DaisyUI Form Controls

| Blazor | DaisyUI | Clases |
|--------|---------|--------|
| `<InputText>` | `<input class="input">` | `input input-bordered w-full` |
| `<InputSelect>` | `<select class="select">` | `select select-bordered w-full` |
| `<InputCheckbox>` | `<input class="checkbox">` | `checkbox checkbox-primary` |
| `<InputDate>` | `<input type="date">` | `input input-bordered` |
| `<InputTextArea>` | `<textarea class="textarea">` | `textarea textarea-bordered` |
| `<InputFile>` | `<input type="file" class="file-input">` | `file-input file-input-bordered` |
| `<ValidationMessage>` | `<label class="label">` | `label-text-alt text-error` |

---

## Mapeo de Estados Críticos del Negocio

### PINes y Transacciones

| Estado del Negocio | Componente DaisyUI | Clases |
|-------------------|-------------------|--------|
| PIN Activo | `badge` | `badge badge-success` |
| PIN Usado | `badge` | `badge badge-info` |
| PIN Vencido | `badge` | `badge badge-error` |
| PIN Devuelto | `badge` | `badge badge-warning` |
| Transacción Exitosa | `alert` | `alert alert-success` |
| Transacción Fallida | `alert` | `alert alert-error` |
| Transacción Pendiente | `alert` | `alert alert-warning` |
| Pago PSE en proceso | `loading` + `alert` | `alert alert-info` + `loading` |

### SuperTransporte

| Estado del Negocio | Componente DaisyUI | Clases |
|-------------------|-------------------|--------|
| Alerta Temprana | `alert` | `alert alert-error` 🔴 |
| SOAT vencido | `badge` | `badge badge-error` |
| Resolución vigente | `badge` | `badge badge-success` |
| Resolución próxima a vencer | `badge` | `badge badge-warning` |
| Documentación incompleta | `alert` | `alert alert-warning` |
| Centro habilitado | `badge` | `badge badge-success badge-lg` |
| Centro suspendido | `badge` | `badge badge-error badge-lg` |
| Información pendiente | `badge` | `badge badge-outline` |

### Agendamiento

| Estado del Negocio | Componente DaisyUI | Clases |
|-------------------|-------------------|--------|
| Cita programada | `badge` | `badge badge-info` |
| Cita completada | `badge` | `badge badge-success` |
| Cita cancelada | `badge` | `badge badge-error` |
| Cupo disponible | `badge` | `badge badge-success badge-sm` |
| Cupo agotado | `badge` | `badge badge-error badge-sm` |
| Horario bloqueado | `badge` | `badge badge-neutral` |

### Facturación

| Estado del Negocio | Componente DaisyUI | Clases |
|-------------------|-------------------|--------|
| Factura emitida | `badge` | `badge badge-success` |
| Factura anulada | `badge` | `badge badge-error` |
| Factura pendiente | `badge` | `badge badge-warning` |
| Nota crédito | `badge` | `badge badge-info` |

---

## Checklist de Accesibilidad (WCAG 2.1 AA)

### 1. Navegación por Teclado

- [ ] **Tab order lógico** — Todos los elementos interactivos son alcanzables con Tab
- [ ] **Focus visible** — Ring de enfoque visible en todos los elementos focusables
  ```css
  /* DaisyUI incluye esto por defecto */
  :focus-visible {
    outline: 2px solid hsl(var(--p));
    outline-offset: 2px;
  }
  ```
- [ ] **Skip to content** — Link "Saltar al contenido" al inicio de la página
  ```svelte
  <a href="#main-content" class="sr-only focus:not-sr-only focus:absolute focus:z-50
    focus:p-4 focus:bg-primary focus:text-primary-content">
    Saltar al contenido principal
  </a>
  ```
- [ ] **Escape para cerrar** — Modales y dropdowns se cierran con Escape
- [ ] **Arrow keys** — Navegación con flechas en menús y tabs
- [ ] **Enter/Space** — Activación de botones y links

### 2. Etiquetas y ARIA

- [ ] **Labels en inputs** — Todos los `<input>` tienen `<label>` asociado
  ```svelte
  <label class="label" for="email">
    <span class="label-text">Correo electrónico</span>
  </label>
  <input id="email" type="email" class="input input-bordered" required />
  ```
- [ ] **aria-label en iconos** — Botones solo con icono tienen `aria-label`
  ```svelte
  <button class="btn btn-ghost" aria-label="Cerrar menú">
    <svg>...</svg>
  </button>
  ```
- [ ] **aria-expanded** — Menús colapsables indican su estado
  ```svelte
  <button aria-expanded={isOpen} aria-controls="submenu-1">
    Configuración
  </button>
  <ul id="submenu-1" class:hidden={!isOpen}>...</ul>
  ```
- [ ] **aria-current="page"** — Link activo en navegación
  ```svelte
  <a href="/pines" aria-current={$page.url.pathname === '/pines' ? 'page' : undefined}>
    PINes
  </a>
  ```
- [ ] **role="alert"** — Mensajes de error y notificaciones
  ```svelte
  <div class="alert alert-error" role="alert">
    <span>Error al guardar</span>
  </div>
  ```

### 3. Contraste de Color

| Elemento | Foreground | Background | Ratio | Cumple AA |
|----------|-----------|------------|-------|-----------|
| Texto principal | `#111827` | `#FFFFFF` | 18.4:1 | ✅ |
| Texto sobre primary | `#FFFFFF` | `#1E3A5F` | 10.1:1 | ✅ |
| Texto sobre success | `#FFFFFF` | `#10B981` | 3.3:1 | ⚠️ (usar texto oscuro) |
| Texto sobre warning | `#111827` | `#F59E0B` | 8.2:1 | ✅ |
| Texto sobre error | `#FFFFFF` | `#EF4444` | 4.6:1 | ✅ |
| Texto secundario | `#6B7280` | `#FFFFFF` | 5.0:1 | ✅ |
| Placeholder | `#9CA3AF` | `#FFFFFF` | 3.0:1 | ⚠️ (revisar) |

**Acción requerida:** Ajustar `success-content` a texto oscuro para mejor contraste.

### 4. Lectores de Pantalla

- [ ] **Estructura semántica** — Usar `<nav>`, `<main>`, `<aside>`, `<header>`, `<footer>`
  ```svelte
  <header>
    <NavPrimary />
  </header>
  <aside>
    <NavSidebar />
  </aside>
  <main id="main-content">
    <slot />
  </main>
  ```
- [ ] **Headings jerárquicos** — `h1` → `h2` → `h3` sin saltar niveles
- [ ] **alt text en imágenes** — Todas las imágenes tienen `alt` descriptivo
- [ ] **Tablas accesibles** — `<caption>`, `<th scope="col">`, `<th scope="row">`
  ```svelte
  <table class="table">
    <caption class="sr-only">Lista de PINes activos</caption>
    <thead>
      <tr>
        <th scope="col">Código</th>
        <th scope="col">Estado</th>
      </tr>
    </thead>
  </table>
  ```
- [ ] **Live regions** — Actualizaciones dinámicas anunciadas
  ```svelte
  <div aria-live="polite" aria-atomic="true" class="sr-only">
    {statusMessage}
  </div>
  ```

### 5. Formularios Accesibles

- [ ] **Errores vinculados** — `aria-describedby` conecta input con error
  ```svelte
  <input id="doc" aria-describedby="doc-error" aria-invalid={!!error} class="input"
    class:input-error={!!error} />
  {#if error}
    <p id="doc-error" class="text-error text-sm mt-1">{error}</p>
  {/if}
  ```
- [ ] **Required indicado** — Campos obligatorios marcados visualmente y con `required`
- [ ] **Grupo de radio/checkbox** — Usar `<fieldset>` + `<legend>`
- [ ] **Autocompletado** — Atributos `autocomplete` en campos de datos personales

---

## Estrategia Mobile-First

### Breakpoints

| Breakpoint | Tailwind | Uso |
|-----------|---------|-----|
| < 640px | Default (mobile) | Sidebar oculto, layout de una columna |
| ≥ 640px | `sm:` | Ajustes menores |
| ≥ 768px | `md:` | Sidebar visible, grids de 2 columnas |
| ≥ 1024px | `lg:` | Layout completo, grids de 3+ columnas |
| ≥ 1280px | `xl:` | Contenido más amplio |

### Componentes Responsivos

| Componente | Mobile | Desktop |
|-----------|--------|---------|
| AdminShell | Drawer (hamburger) | Sidebar fijo |
| NavSidebar | Overlay drawer | Sidebar lateral |
| DynamicGrid | Scroll horizontal + cards | Tabla completa |
| Modal | Full-screen | Centered dialog |
| FormField | Full-width stack | Inline labels |
| Tabs | Scroll horizontal | Tabs visibles |

### Ejemplo: Sidebar Responsivo

```svelte
<!-- AdminShell.svelte -->
<div class="drawer lg:drawer-open">
  <input id="sidebar-drawer" type="checkbox" class="drawer-toggle" bind:checked={drawerOpen} />

  <!-- Contenido principal -->
  <div class="drawer-content">
    <header class="navbar bg-base-100 lg:hidden">
      <label for="sidebar-drawer" class="btn btn-ghost drawer-button" aria-label="Abrir menú">
        <svg><!-- hamburger icon --></svg>
      </label>
    </header>
    <main id="main-content" class="p-4">
      <slot />
    </main>
  </div>

  <!-- Sidebar -->
  <div class="drawer-side">
    <label for="sidebar-drawer" class="drawer-overlay" aria-label="Cerrar menú"></label>
    <NavSidebar />
  </div>
</div>
```

### Ejemplo: Tabla Responsiva

```svelte
<!-- En mobile: cards apiladas. En desktop: tabla -->
<div class="hidden md:block">
  <DynamicGrid {columns} {data} />
</div>
<div class="md:hidden space-y-2">
  {#each data as row}
    <div class="card bg-base-100 shadow-sm p-4">
      {#each columns as col}
        <div class="flex justify-between">
          <span class="font-semibold text-sm">{col.label}</span>
          <span>{row[col.key]}</span>
        </div>
      {/each}
    </div>
  {/each}
</div>
```
