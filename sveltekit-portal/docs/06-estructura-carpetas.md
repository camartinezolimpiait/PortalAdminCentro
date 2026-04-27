# Estructura de Carpetas SvelteKit

## Árbol Completo con Anotaciones

```
sveltekit-portal/
├── docs/                                    # 📚 Documentación de migración (este directorio)
│   ├── 01-diagnostico-arquitectura.md       #    Diagnóstico del proyecto Blazor original
│   ├── 02-mapa-rutas.md                     #    Mapeo de rutas Blazor → SvelteKit
│   ├── 03-matriz-componentes.md             #    Mapeo de componentes .razor → .svelte
│   ├── 04-matriz-servicios.md               #    Mapeo de servicios C# → TypeScript
│   ├── 05-implementacion-rbac.md            #    Implementación RBAC en hooks
│   ├── 06-estructura-carpetas.md            #    Este archivo
│   ├── 07-log-cambios.md                    #    Log de cambios detallado
│   ├── 08-conversion-tipos.md               #    Conversión de tipos C# → TypeScript
│   ├── 09-reemplazo-di.md                   #    Reemplazo de DI .NET → SvelteKit
│   ├── 10-tematizacion-accesibilidad.md     #    Temas y accesibilidad con DaisyUI
│   └── 11-riesgos-decisiones.md             #    Riesgos, supuestos y decisiones
│
├── src/                                     # 📂 Código fuente de la aplicación
│   ├── app.html                             # 🏠 Template HTML raíz (equivalente a _Host.cshtml)
│   ├── app.css                              # 🎨 Estilos globales (Tailwind + DaisyUI imports)
│   ├── app.d.ts                             # 📝 Tipos globales (App.Locals, App.PageData)
│   │
│   ├── hooks.server.ts                      # 🔒 Middleware server-side:
│   │                                        #    - Lee cookie de sesión
│   │                                        #    - Hidrata event.locals.user
│   │                                        #    - Verifica acceso a rutas (RBAC)
│   │                                        #    - Redirige si no autorizado
│   │
│   ├── lib/                                 # 📦 Código compartido ($lib alias)
│   │   ├── index.ts                         #    Barrel export principal
│   │   │
│   │   ├── assets/                          # 🖼️ Assets estáticos
│   │   │   └── favicon.svg                  #    Favicon de la aplicación
│   │   │
│   │   ├── server/                          # 🔐 Módulos SOLO server (nunca llegan al cliente)
│   │   │   ├── auth.ts                      #    Autenticación: obtenerToken(), obtenerPerfil()
│   │   │   │                                #    Reemplaza: ITokenService + IPerfilService
│   │   │   ├── session.ts                   #    Codificación/decodificación de sesión (base64url)
│   │   │   │                                #    Reemplaza: ProtectedSessionStorage
│   │   │   ├── rbac.ts                      #    Control de acceso basado en roles
│   │   │   │                                #    Reemplaza: ApplicationSevice.Autenticado + chequeos en layouts
│   │   │   ├── api-client.ts                #    Cliente HTTP genérico con factory pattern
│   │   │   │                                #    Reemplaza: IApiService + HttpClient DI
│   │   │   └── constants.ts                 #    Constantes del servidor (URLs, keys)
│   │   │                                    #    Reemplaza: appsettings.json + IOptions<>
│   │   │
│   │   ├── components/                      # 🧩 Componentes Svelte reutilizables
│   │   │   ├── layout/                      # ── Layouts ──
│   │   │   │   └── AdminShell.svelte        #    Shell principal: sidebar + navbar + contenido
│   │   │   │                                #    Reemplaza: PortalLayout.razor
│   │   │   │
│   │   │   ├── navigation/                  # ── Navegación ──
│   │   │   │   ├── NavPrimary.svelte        #    Barra de navegación superior
│   │   │   │   │                            #    Reemplaza: NavMenuPrincipal.razor
│   │   │   │   └── NavSidebar.svelte        #    Menú lateral con items por rol
│   │   │   │                                #    Reemplaza: NavMenuPortal.razor + NavSubMenuPrincipal.razor
│   │   │   │
│   │   │   ├── ui/                          # ── Componentes UI genéricos ──
│   │   │   │   ├── AlertMessage.svelte      #    Alertas con variantes DaisyUI
│   │   │   │   │                            #    Reemplaza: AlertMessage.razor
│   │   │   │   ├── BtnLoader.svelte         #    Botón con estado de carga
│   │   │   │   │                            #    Reemplaza: BtnLoader.razor + BtnLoaderSuper.razor
│   │   │   │   ├── Modal.svelte             #    Modal con backdrop y slots
│   │   │   │   │                            #    Reemplaza: Todos los modales individuales
│   │   │   │   └── ToastContainer.svelte    #    Contenedor de notificaciones toast
│   │   │   │                                #    Reemplaza: Blazored.Toast
│   │   │   │
│   │   │   ├── tables/                      # ── Tablas y grillas ──
│   │   │   │   └── DynamicGrid.svelte       #    Tabla dinámica con sort + paginación
│   │   │   │                                #    Reemplaza: GridDinamico.razor + QuickGrid
│   │   │   │
│   │   │   └── forms/                       # ── Formularios ──
│   │   │       └── FormField.svelte         #    Campo de formulario con label + error
│   │   │                                    #    Reemplaza: EditForm pattern + InputText + ValidationMessage
│   │   │
│   │   ├── types/                           # 📋 Definiciones TypeScript
│   │   │   ├── index.ts                     #    Barrel export de tipos
│   │   │   ├── auth.ts                      #    UserSession, LoginRequest, TokenResponse
│   │   │   │                                #    Reemplaza: TokenModel, ApplicationSevice props
│   │   │   ├── api.ts                       #    ApiResponse<T>, PaginatedResponse<T>
│   │   │   │                                #    Reemplaza: RespuestaServicios<T>, RespGeneralModel
│   │   │   ├── pines.ts                     #    Pin, PinFiltros, PinEstado
│   │   │   │                                #    Reemplaza: ActivosFiltrosModel, ValidaPinPaynetModel
│   │   │   ├── compra-pin.ts                #    CompraRequest, DatosBasicos, MedioPago
│   │   │   │                                #    Reemplaza: CompraPinFormModel, DatosBasicosModel, etc.
│   │   │   ├── agendamiento.ts              #    Cita, HorarioAtencion, Agenda
│   │   │   │                                #    Reemplaza: Modelos de agendamiento
│   │   │   ├── supertransporte.ts           #    InfoBasica, Poliza, ResolucionHabilitacion
│   │   │   │                                #    Reemplaza: Todos los DTOs de SuperTransporte
│   │   │   └── facturacion.ts               #    Factura, ConfiguracionFacturacion
│   │   │                                    #    Reemplaza: Modelos de facturación
│   │   │
│   │   └── stores/                          # 🏪 Svelte stores (estado reactivo cliente)
│   │       └── toast.ts                     #    Store para notificaciones toast
│   │                                        #    Reemplaza: IToastService (Blazored.Toast)
│   │
│   └── routes/                              # 🛣️ Rutas de la aplicación (filesystem-based)
│       ├── +layout.svelte                   #    Layout raíz: lee $page.data.user, decide layout
│       ├── +layout.server.ts                #    Server load raíz: expone user a $page.data
│       ├── +page.svelte                     #    Página raíz: redirect a /login o /pines
│       ├── +error.svelte                    #    Página de error global
│       │
│       ├── login/                           # ── Autenticación ──
│       │   ├── +page.svelte                 #    Formulario de login
│       │   └── +page.server.ts              #    Form Action: autenticar y set cookie
│       │
│       ├── 403-unauthorized/                # ── Acceso denegado ──
│       │   └── +page.svelte                 #    Página 403
│       │
│       ├── change-password/                 # ── Cambio de contraseña ──
│       │   └── +page.svelte                 #    Formulario de cambio de contraseña
│       │
│       ├── api/                             # ── API Routes ──
│       │   └── auth/
│       │       └── logout/
│       │           └── +server.ts           #    POST: eliminar cookie de sesión
│       │
│       ├── pines/                           # ── Gestión de PINes ──
│       │   ├── +page.svelte                 #    Vista principal con tabs por estado
│       │   └── +page.server.ts              #    Server load: fetch pines del API
│       │
│       ├── compradepin/                     # ── Compra de PIN (CRC) ──
│       │   └── +page.svelte                 #    Flujo de compra de PIN
│       │
│       ├── comprapincda/                    # ── Compra de PIN (CDA) ──
│       │   └── +page.svelte                 #    Flujo de compra PIN CDA
│       │
│       ├── configuracion/                   # ── Configuración ──
│       │   ├── +page.svelte                 #    Dashboard de configuración
│       │   └── +layout.svelte               #    Layout con tabs de configuración
│       │
│       ├── consultar-facturacion/           # ── Consulta de Facturación ──
│       │   └── +page.svelte                 #    Consulta y filtrado de facturas
│       │
│       ├── agenda/                          # ── Agendamiento ──
│       │   └── +page.svelte                 #    Vista de agenda/calendario
│       │
│       ├── supertransporte/                 # ── SuperTransporte ──
│       │   ├── centro/                      #    CRC (Centro de Reconocimiento)
│       │   │   ├── +page.svelte             #    Dashboard CRC
│       │   │   └── +layout.svelte           #    Layout con nav lateral CRC
│       │   └── centro-cea/                  #    CEA (Centro de Enseñanza)
│       │       ├── +page.svelte             #    Dashboard CEA
│       │       └── +layout.svelte           #    Layout con nav lateral CEA
│       │
│       └── reportes/                        # ── Reportes PowerBI ──
│           └── +page.svelte                 #    Embebido de reportes PowerBI
│
├── static/                                  # 📁 Archivos estáticos (servidos directamente)
│   └── (favicon, imágenes, fuentes)
│
├── package.json                             # 📦 Dependencias npm
├── svelte.config.js                         # ⚙️ Configuración de SvelteKit
├── tailwind.config.js                       # 🎨 Configuración de Tailwind + DaisyUI
├── tsconfig.json                            # 📝 Configuración TypeScript
├── vite.config.ts                           # ⚡ Configuración de Vite
└── README.md                                # 📖 Documentación del proyecto
```

---

## Convenciones de Archivos

| Archivo | Propósito | Ejecuta en |
|---------|-----------|-----------|
| `+page.svelte` | Componente de página (UI) | Cliente + SSR |
| `+page.server.ts` | Server load + Form Actions | Solo servidor |
| `+page.ts` | Universal load (cliente + server) | Ambos |
| `+layout.svelte` | Layout compartido para grupo de rutas | Cliente + SSR |
| `+layout.server.ts` | Server load para layout | Solo servidor |
| `+server.ts` | API endpoint (GET, POST, etc.) | Solo servidor |
| `+error.svelte` | Página de error para el grupo | Cliente + SSR |

## Convención de Imports

| Alias | Ruta Real | Uso |
|-------|-----------|-----|
| `$lib` | `src/lib` | Componentes, tipos, stores |
| `$lib/server` | `src/lib/server` | Módulos solo-servidor (auth, API, RBAC) |
| `$env/dynamic/private` | Variables de entorno | Secretos (API keys, URLs) |
| `$env/dynamic/public` | Variables de entorno públicas | Config no sensible |
| `$app/stores` | SvelteKit stores | `page`, `navigating`, `updated` |
| `$app/navigation` | Navegación programática | `goto()`, `invalidate()` |

---

## Mapeo de Directorios: Blazor → SvelteKit

| Directorio Blazor | Directorio SvelteKit | Notas |
|-------------------|---------------------|-------|
| `Pages/` | `src/routes/` | Filesystem routing |
| `Shared/` | `src/lib/components/` | Componentes reutilizables |
| `Application/Services/` | `src/lib/server/` | Lógica de negocio server-side |
| `Application/Models/` | `src/lib/types/` | Definiciones de tipos |
| `wwwroot/` | `static/` | Assets estáticos |
| `wwwroot/css/` | `src/app.css` | Estilos globales (Tailwind) |
| `_Imports.razor` | `src/lib/index.ts` | Imports globales |
| `App.razor` | `src/routes/+layout.svelte` | Router principal |
| `appsettings.json` | `.env` + `$env/dynamic/private` | Configuración |
| `Startup.cs` | `hooks.server.ts` + `svelte.config.js` | Arranque y middleware |
