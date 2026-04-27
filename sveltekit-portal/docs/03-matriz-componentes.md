# Matriz de Componentes .razor → .svelte

## Resumen de Migración

| Categoría | Componentes Blazor | Componentes Svelte | Estado |
|-----------|-------------------|-------------------|--------|
| Layouts | 7 | 3 (consolidados) | ✅ Creados |
| Navegación | 3 | 2 | ✅ Creados |
| UI Compartidos | 6 | 5 | ✅ Creados |
| Formularios | EditForm pattern | Form Actions + FormField | ✅ Creados |
| Tablas/Grillas | 2 | 1 (consolidado) | ✅ Creado |
| Páginas | ~80 | ~25 (fase inicial) | 🔄 En progreso |

---

## 1. Layouts

| Blazor (.razor) | SvelteKit (.svelte) | Ubicación SvelteKit | Notas |
|-----------------|--------------------|--------------------|-------|
| `PortalLayout.razor` | `AdminShell.svelte` | `$lib/components/layout/` | Layout principal con sidebar, navbar, contenido. Incluye drawer para mobile. |
| `LoginLayout.razor` | _(inline en `+layout.svelte`)_ | `src/routes/login/` | No necesita componente separado; el layout de login se define directo en la ruta. |
| `SupertransporteLayout.razor` | `+layout.svelte` | `src/routes/supertransporte/centro/` y `centro-cea/` | Layout de grupo con navegación lateral específica de SuperTransporte. |
| `PowerBILayout.razor` | `+layout.svelte` | `src/routes/reportes/` | Layout simplificado para embebido de PowerBI. |
| `TwoPaneLayout.razor` | _(CSS grid en page)_ | — | Se reemplaza por clases utilitarias de Tailwind (`grid grid-cols-2`). |
| `FormularioLayout.razor` | _(inline en pages)_ | — | Se integra como sección dentro del `AdminShell`. |
| `SingleLayout.razor` | _(slot en `+layout.svelte`)_ | — | Layout con una columna; se logra con `max-w-xl mx-auto`. |

### Diagrama de Jerarquía de Layouts

```
src/routes/
├── +layout.svelte              ← Root layout (lee $page.data.user)
│   ├── login/+layout.svelte    ← LoginLayout (sin sidebar)
│   ├── +layout.svelte          ← AdminShell (sidebar + navbar)
│   │   ├── pines/
│   │   ├── configuracion/
│   │   │   └── +layout.svelte  ← Tabs de configuración
│   │   ├── supertransporte/
│   │   │   ├── centro/+layout.svelte    ← SupertransporteLayout CRC
│   │   │   └── centro-cea/+layout.svelte ← SupertransporteLayout CEA
│   │   ├── reportes/           ← PowerBILayout
│   │   └── ...
│   └── enrollment/             ← FormularioLayout (público)
```

---

## 2. Componentes de Navegación

| Blazor (.razor) | SvelteKit (.svelte) | Ubicación | Props/Comportamiento |
|-----------------|--------------------|-----------|--------------------|
| `NavMenuPrincipal.razor` | `NavPrimary.svelte` | `$lib/components/navigation/` | Barra superior con logo, menú principal, usuario, logout. Reactivo a `$page.data.user`. |
| `NavMenuPortal.razor` | `NavSidebar.svelte` | `$lib/components/navigation/` | Menú lateral con items colapsables. Filtra items por rol del usuario. Mobile: drawer. |
| `NavSubMenuPrincipal.razor` | _(integrado en NavSidebar)_ | — | Los submenús se modelan como items anidados dentro del sidebar. |

---

## 3. Componentes UI Compartidos

| Blazor (.razor) | SvelteKit (.svelte) | Ubicación | Equivalencia |
|-----------------|--------------------|-----------:|-------------|
| `AlertMessage.razor` | `AlertMessage.svelte` | `$lib/components/ui/` | DaisyUI `alert` con variantes: `alert-info`, `alert-success`, `alert-warning`, `alert-error`. Props: `type`, `message`, `dismissible`. |
| `BtnLoader.razor` | `BtnLoader.svelte` | `$lib/components/ui/` | DaisyUI `btn` + `loading loading-spinner` cuando `loading=true`. Props: `loading`, `disabled`, `label`, `type`. |
| `BtnLoaderSuper.razor` | `BtnLoader.svelte` | _(consolidado)_ | Se unifica con BtnLoader usando variantes de estilo via prop `variant`. |
| `GridDinamico.razor` | `DynamicGrid.svelte` | `$lib/components/tables/` | Tabla con DaisyUI `table` + paginación + ordenamiento. Props: `columns`, `data`, `sortable`, `paginated`. Reemplaza QuickGrid. |
| `DynamicQuickGrid.razor` | `DynamicGrid.svelte` | _(consolidado)_ | Se unifica con DynamicGrid; QuickGrid no existe en Svelte. |
| _(Blazored.Toast)_ | `ToastContainer.svelte` | `$lib/components/ui/` | Store reactivo `$lib/stores/toast.ts` + DaisyUI `toast` + `alert`. Auto-dismiss configurable. |
| _(Modal genérico)_ | `Modal.svelte` | `$lib/components/ui/` | DaisyUI `modal` con backdrop click-to-close. Props: `open`, `title`, `size`. Slot para contenido. |

---

## 4. Componentes de Formularios

| Blazor Patrón | SvelteKit Equivalente | Archivo | Notas |
|--------------|----------------------|---------|-------|
| `<EditForm Model="@model">` | `<form method="POST" use:enhance>` | Nativo SvelteKit | Form Actions para submit server-side |
| `<InputText @bind-Value="model.Name">` | `<FormField>` | `$lib/components/forms/FormField.svelte` | Wrapper con label, input, error message. Bind via `bind:value`. |
| `<InputSelect>` | `<select class="select">` | DaisyUI nativo | Con clases DaisyUI `select select-bordered` |
| `<InputCheckbox>` | `<input type="checkbox" class="checkbox">` | DaisyUI nativo | Con clases DaisyUI |
| `<InputDate>` | `<input type="date" class="input">` | DaisyUI nativo | Bind directo |
| `<ValidationMessage>` | Error display en `FormField` | — | Errores de `ActionData` se pasan como props |
| `<DataAnnotationsValidator>` | Zod / server validation | `+page.server.ts` | Validación en el server con schemas Zod |
| `EditContext.OnValidSubmit` | `export const actions = { default: ... }` | `+page.server.ts` | Lógica de validación y procesamiento server-side |

---

## 5. Componentes de Módulo — Pines

| Blazor (.razor) | SvelteKit (.svelte) | Ruta | Estado |
|-----------------|--------------------|---------:|--------|
| `Pines.razor` | `+page.svelte` | `/pines` | ✅ Creado |
| `Activos.razor` (tab) | _(integrado)_ | `/pines?estado=activos` | 🔄 Tab dentro de page |
| `Usados.razor` (tab) | _(integrado)_ | `/pines?estado=usados` | 🔄 Tab dentro de page |
| `Devoluciones.razor` (tab) | _(integrado)_ | `/pines?estado=devoluciones` | 🔄 Tab dentro de page |
| `Dispersiones.razor` (tab) | _(integrado)_ | `/pines?estado=dispersiones` | 🔄 Tab dentro de page |
| `Busqueda.razor` (tab) | _(integrado)_ | `/pines?estado=busqueda` | 🔄 Tab dentro de page |
| `PinesAsociados.razor` (tab) | _(integrado)_ | `/pines?estado=asociados` | 🔄 Tab dentro de page |
| `ModalPinesAsociados.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |
| `DetalleTransaccionModal.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |
| `CertificadoIngreso.razor` | `+page.svelte` | `/pines/certificado-ingreso` | 📋 Pendiente |

---

## 6. Componentes de Módulo — CompraPin

| Blazor (.razor) | SvelteKit (.svelte) | Ruta | Estado |
|-----------------|--------------------|---------:|--------|
| `Compra-de-pin.razor` | `+page.svelte` | `/compradepin` | ✅ Stub |
| `DatosBasicos.razor` | `+page.svelte` | `/compradepin/datos-basicos` | 📋 Pendiente |
| `Categorias.razor` | `+page.svelte` | `/compradepin/categorias` | 📋 Pendiente |
| `SeleccionCategorias.razor` | `+page.svelte` | `/compradepin/seleccion-categorias` | 📋 Pendiente |
| `TipoTramite.razor` | `+page.svelte` | `/compradepin/tipo-tramite` | 📋 Pendiente |
| `DatosPersonales.razor` | `+page.svelte` | `/compradepin/datos-personales` | 📋 Pendiente |
| `MediosDePago.razor` | `+page.svelte` | `/compradepin/medios-de-pago` | 📋 Pendiente |
| `ResumenCompra.razor` | `+page.svelte` | `/compradepin/resumen-compra` | 📋 Pendiente |
| `ConfirmacionCompra.razor` | `+page.svelte` | `/compradepin/confirmacion-compra` | 📋 Pendiente |
| `CouponCode.razor` | _(inline)_ | — | 📋 Componente inline |
| `CategoriaSelector.razor` | _(inline)_ | — | 📋 Componente inline |
| `ModalGeneracionPin.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |
| `ModalPseColpatria.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |
| `GoHomeModal.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |
| `Blog.razor` | `+page.svelte` | `/compradepin/blog` | 📋 Pendiente |
| `Articulo.razor` | `+page.svelte` | `/compradepin/blog/[slug]` | 📋 Pendiente |

---

## 7. Componentes de Módulo — Agendamiento

| Blazor (.razor) | SvelteKit (.svelte) | Ruta | Estado |
|-----------------|--------------------|---------:|--------|
| `Agenda.razor` | `+page.svelte` | `/agenda` | ✅ Stub |
| `NewAppoimentComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `ReScheduleAppoimentComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `ReScheduleConfirmationComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `CancelAppoimentComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `BlockScheduleComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `AppoimentsComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `AppoimentDetailsComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |
| `ButtonBlockActionsComponent.razor` | _(componente inline)_ | — | 📋 Pendiente |

---

## 8. Componentes de Módulo — SuperTransporte

| Blazor (.razor) | SvelteKit (.svelte) | Ruta | Estado |
|-----------------|--------------------|---------:|--------|
| `CentroSuper.razor` | `+page.svelte` | `/supertransporte/centro` | ✅ Stub |
| `InfoBasica.razor` | `+page.svelte` | `/supertransporte/centro/informacion-basica` | 📋 Pendiente |
| `InfoProfesionalesSalud.razor` | `+page.svelte` | `/supertransporte/centro/profesional-salud` | 📋 Pendiente |
| _(13 sub-páginas CRC más)_ | `+page.svelte` cada una | `/supertransporte/centro/...` | 📋 Pendiente |
| `CentroSuperCEA.razor` | `+page.svelte` | `/supertransporte/centro-cea` | ✅ Stub |
| _(14 sub-páginas CEA)_ | `+page.svelte` cada una | `/supertransporte/centro-cea/...` | 📋 Pendiente |

---

## 9. Componentes de Módulo — Facturación

| Blazor (.razor) | SvelteKit (.svelte) | Ruta | Estado |
|-----------------|--------------------|---------:|--------|
| `ConfigurarFacturacion.razor` | `+page.svelte` | `/configuracion/facturacion` | 📋 Pendiente |
| `Estado.razor` | `+page.svelte` | `/configuracion/facturacion/estado` | 📋 Pendiente |
| `Emision.razor` | `+page.svelte` | `/configuracion/facturacion/emision` | 📋 Pendiente |
| `Articulos.razor` | `+page.svelte` | `/configuracion/facturacion/articulos` | 📋 Pendiente |
| `Credenciales.razor` | `+page.svelte` | `/configuracion/facturacion/credenciales` | 📋 Pendiente |
| `Comportamiento.razor` | `+page.svelte` | `/configuracion/facturacion/comportamiento` | 📋 Pendiente |
| `Numeracion.razor` | `+page.svelte` | `/configuracion/facturacion/numeracion` | 📋 Pendiente |
| `ConsultarFacturacion.razor` | `+page.svelte` | `/consultar-facturacion` | ✅ Stub |
| `ModalDetalleFacturacion.razor` | `Modal.svelte` (reutilizado) | — | 📋 Pendiente |

---

## 10. Leyenda de Estados

| Icono | Significado |
|-------|------------|
| ✅ | Componente creado y funcional |
| 🔄 | En progreso / parcialmente implementado |
| 📋 | Pendiente de implementación |
| ❌ | No se migrará (eliminado) |
