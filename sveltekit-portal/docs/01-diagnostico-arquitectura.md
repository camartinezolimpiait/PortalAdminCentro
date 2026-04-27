# Diagnóstico de Arquitectura Actual

## Tipo de Aplicación

**Blazor Server (.NET 10)** — No es WebAssembly ni híbrido.

### Evidencia Técnica

| Indicador | Archivo | Hallazgo |
|-----------|---------|----------|
| `MapBlazorHub()` | `Program.cs:37` | Hub de SignalR para Blazor Server |
| `AddServerSideBlazor()` | `Startup.cs` | Registro de servicios server-side |
| `_Host.cshtml` | `Pages/_Host.cshtml` | Punto de entrada server-rendered |
| `ProtectedSessionStorage` | Múltiples archivos | Almacenamiento de sesión protegido (server) |
| `render-mode="Server"` | `_Host.cshtml` | Modo de renderizado explícito |
| **No** `WebAssemblyHostBuilder` | — | Descarta WASM |
| **No** proyecto `.Client` | — | Descarta modelo híbrido |

### Diagrama de Arquitectura

```
┌──────────────┐     WebSocket (SignalR)     ┌─────────────────────┐
│              │◄───────────────────────────►│                     │
│   Browser    │    HTML Diffs (DOM Patches)  │   ASP.NET Core      │
│   (thin)     │                             │   Blazor Server     │
│              │    User Events (clicks,      │                     │
│   - No .NET  │    keystrokes, etc.)        │   - 134 .razor      │
│   - No WASM  │                             │   - 17+ services    │
│              │                             │   - Session state    │
└──────────────┘                             └─────────┬───────────┘
                                                       │
                                              HTTP / REST API
                                                       │
                                             ┌─────────▼───────────┐
                                             │   External APIs     │
                                             │   - SISEC Backend   │
                                             │   - MiLicencia      │
                                             │   - Paynet/Wompi    │
                                             │   - PowerBI         │
                                             │   - Azure Blob      │
                                             └─────────────────────┘
```

## Métricas del Proyecto

| Métrica | Valor |
|---------|-------|
| Archivos `.razor` | ~134 (páginas + componentes) |
| Archivos `.cs` (servicios) | 19+ interfaces de servicio |
| DTOs / Modelos | 105+ clases |
| Layouts | 7 (Portal, Login, SuperTransporte, PowerBI, TwoPane, Formulario, Single) |
| Componentes de navegación | 3 (NavMenuPrincipal, NavMenuPortal, NavSubMenuPrincipal) |
| Rutas `@page` | ~80 directivas |
| Registros DI | 20+ servicios |

## Dependencias Principales (NuGet)

| Paquete | Versión | Propósito | Equivalente SvelteKit |
|---------|---------|-----------|----------------------|
| `Blazored.Toast` | 3.2.2 | Notificaciones toast | DaisyUI alerts / toast store |
| `BlazorPro.Spinkit` | 1.2.0 | Spinners de carga | CSS/DaisyUI loading |
| `ClosedXML` | 0.105.0 | Manipulación Excel | SheetJS (xlsx) o server-side |
| `EPPlus` | 7.1.2 | Generación Excel | SheetJS (xlsx) o server-side |
| `Microsoft.PowerBI.Api` | 4.14.0 | Embebido PowerBI | PowerBI JS SDK |
| `PuppeteerSharp` | 2.0.4 | Automatización headless | Playwright (si necesario) |
| `QuickGrid` | 10.0.7 | Grilla de datos | Componente DynamicGrid.svelte |
| `Newtonsoft.Json` | 13.0.4 | Serialización JSON | Nativo (`JSON.parse/stringify`) |
| `Azure.Storage.Blobs` | 12.27.0 | Almacenamiento Azure | `@azure/storage-blob` |
| `BlazorInputFile` | 0.2.0 | Input de archivos | `<input type="file">` nativo |
| `Microsoft.Identity.Client` | 4.83.3 | Azure AD/Identity | MSAL.js o server-side |
| `StackifyMiddleware` | 3.0.5.2 | Monitoreo | OpenTelemetry / Sentry |

## Autenticación y Autorización

### Esquema de Autenticación

```
Tipo:            Cookie Authentication
Nombre cookie:   .SISEC.Session
SameSite:        None (permite cross-site)
Secure:          Always (solo HTTPS)
IsEssential:     true
Session Timeout: 10 segundos (!!!)
```

### Problemas Detectados

1. **Sin `[Authorize]` en rutas** — La protección se hace via `ApplicationSevice.Autenticado` (flag booleano) evaluado en los layouts, no a nivel de middleware/pipeline.

2. **Timeout de sesión extremadamente bajo** — 10 segundos es inusual; probablemente un error de configuración o un mecanismo de "keep-alive" mediante heartbeat de SignalR.

3. **Token en almacenamiento del browser** — `ProtectedSessionStorage` mantiene el token en el navegador (cifrado, pero accesible al servidor via SignalR).

4. **Sin protección de rutas server-side** — Cualquier ruta puede ser accedida si se navega directamente; la protección depende del layout.

5. **SameSite=None** — Potencial riesgo de CSRF sin mitigaciones adicionales.

## Deuda Técnica Identificada

| Área | Problema | Severidad |
|------|----------|-----------|
| Seguridad | Sin protección de rutas a nivel de servidor | 🔴 Alta |
| Seguridad | Timeout de sesión = 10s | 🟡 Media |
| Seguridad | Token almacenado en browser storage | 🟡 Media |
| Seguridad | SameSite=None sin protección CSRF | 🟡 Media |
| Arquitectura | Config duplicada (Startup.cs + StartupExtensions.cs) | 🟡 Media |
| Mantenimiento | 17+ servicios sin tests unitarios evidentes | 🔴 Alta |
| Rendimiento | SignalR WebSocket per-user = alto consumo de recursos | 🟡 Media |
| Código | `ApplicationSevice` (typo en "Service") como estado global | 🟢 Baja |
