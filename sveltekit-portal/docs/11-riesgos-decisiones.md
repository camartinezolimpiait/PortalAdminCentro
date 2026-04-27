# Riesgos, Supuestos y Decisiones de Arquitectura

## 1. Supuestos Explícitos

| # | Supuesto | Impacto si es Falso |
|---|---------|---------------------|
| S1 | La API backend de SISEC devuelve respuestas JSON con estructura `{ estado, mensaje, data, errores }` consistente. | Requiere adaptar `ApiResponse<T>` y posiblemente crear transformadores por endpoint. |
| S2 | Los roles del sistema son: Admin, Auditor, Director, Instructor, Investigador. No hay roles adicionales ni jerárquicos. | Requiere ampliar la tabla RBAC y posiblemente implementar herencia de roles. |
| S3 | El token JWT emitido por la API tiene un tiempo de expiración razonable (≥30 min) y soporta refresh tokens. | Si el timeout es corto o no hay refresh, se necesita implementar re-autenticación silenciosa. |
| S4 | La API backend no requiere afinidad de sesión (es stateless). Cualquier instancia de SvelteKit puede hacer requests con el token. | Si la API tiene estado por sesión, se necesita sticky sessions o migrar estado a cookies/tokens. |
| S5 | Los endpoints de la API son los mismos para CRC y CEA, diferenciados por un parámetro (tipo de centro). | Si son APIs completamente separadas, se necesitan dos api-clients o prefijos de URL distintos. |
| S6 | El esquema de permisos es plano (lista de strings) y no requiere permisos anidados ni condicionales. | Si los permisos son contextuales (ej: "solo sus propios PINes"), se necesita lógica adicional en server loads. |
| S7 | Blazored.Toast, BlazorPro.Spinkit y QuickGrid no tienen lógica de negocio — son puramente UI. | Si alguno tiene side-effects (ej: logging, analytics), se necesita replicar esa funcionalidad. |
| S8 | El flujo de pago con Wompi/PSE es un redirect externo → callback, no un iframe embebido. | Si es iframe, se necesita configurar CSP headers y manejar postMessage. |
| S9 | PowerBI Reports pueden embeberse con el SDK de JavaScript estándar, sin customizaciones server-side especiales. | Si requiere Azure AD intermediario, se necesita un proxy endpoint en SvelteKit. |
| S10 | El deployment será en un solo proceso Node.js (adapter-node), sin necesidad de serverless/edge computing. | Si se requiere edge (Cloudflare Workers, Vercel Edge), hay que cambiar el adapter y revisar APIs de Node.js. |

---

## 2. Riesgos Identificados

| # | Riesgo | Probabilidad | Impacto | Mitigación |
|---|--------|:------------:|:-------:|-----------|
| R1 | **API no documentada** — No existe documentación formal de endpoints, payloads, ni códigos de error de la API backend. | 🔴 Alta | 🔴 Alto | Ingeniería inversa via Blazor code + network inspection. Documentar cada endpoint descubierto. Crear contrato OpenAPI progresivamente. |
| R2 | **Esquema de roles desconocido** — Los roles y permisos exactos no están formalizados; se infieren del código de los layouts. | 🟡 Media | 🔴 Alto | Validar con stakeholders. Implementar RBAC conservador (deny by default). Registrar accesos denegados para detectar gaps. |
| R3 | **Flujo de pago Wompi/PSE** — La integración con pasarelas de pago puede tener requisitos técnicos no visibles en el código Blazor (tokens, webhooks, callbacks). | 🟡 Media | 🔴 Alto | Analizar la documentación de Wompi. Implementar el módulo de pago como último paso, con tests manuales extensivos. |
| R4 | **PowerBI embed token** — La generación de tokens de embebido de PowerBI requiere Azure AD service principal y configuración específica. | 🟡 Media | 🟡 Medio | Mantener el servicio de ReporteService en el servidor. Usar MSAL Node.js para obtener tokens. Probar con un reporte de prueba. |
| R5 | **Pérdida de funcionalidad SignalR** — Blazor Server usa SignalR para actualizaciones en tiempo real. Algunas features pueden depender de push del servidor. | 🟢 Baja | 🟡 Medio | Identificar features que usan real-time (si las hay). Implementar con polling, SSE, o WebSockets nativos si es necesario. |
| R6 | **Regresión visual** — La migración de CSS puede causar diferencias visuales no deseadas. | 🟡 Media | 🟢 Bajo | Screenshots comparativos antes/después. Revisión visual por módulo. DaisyUI themes para consistencia. |
| R7 | **Timeouts y retry** — La API backend puede tener latencias variables; sin retry logic, la UX puede degradarse. | 🟡 Media | 🟡 Medio | Implementar retry con backoff en api-client.ts. Timeout configurable. Loading states en UI. |
| R8 | **Migración de datos de sesión** — Si hay usuarios con sesiones activas al momento del cutover, perderán su sesión. | 🔴 Alta | 🟢 Bajo | Planificar cutover en horario de baja actividad. Notificar usuarios. Las sesiones son de corta duración. |
| R9 | **SEO y deep linking** — Cambio de estructura de URLs puede romper bookmarks o links externos. | 🟢 Baja | 🟢 Bajo | Implementar redirects 301 para las rutas que cambien (ej: `/ChangePassword` → `/change-password`). |
| R10 | **Dependencia de PuppeteerSharp** — Si se usa para generación de PDFs, la migración a Node.js requiere Puppeteer o alternativa. | 🟡 Media | 🟡 Medio | Investigar si PuppeteerSharp se usa en runtime o solo en build. Reemplazar con Playwright o un servicio de generación de PDFs. |

---

## 3. Decisiones de Arquitectura (ADRs)

### ADR-001: Usar adapter-node para deployment

**Contexto:** SvelteKit soporta múltiples adapters (node, vercel, cloudflare, netlify, static).

**Decisión:** Usar `@sveltejs/adapter-node` para generar un servidor Node.js standalone.

**Razón:**
- El proyecto actual usa Docker para deployment
- adapter-node produce un servidor Express/Polka que se ejecuta en cualquier ambiente con Node.js
- Compatible con Docker, Azure App Service, VMs, Kubernetes
- No depende de ningún proveedor cloud específico
- Permite usar APIs de Node.js (crypto, fs) para funcionalidades server-side

**Consecuencias:** Requiere un proceso Node.js corriendo. No es serverless. Necesita health checks y monitoreo de proceso.

---

### ADR-002: HttpOnly cookies en lugar de ProtectedSessionStorage

**Contexto:** Blazor usa `ProtectedSessionStorage` para almacenar el token en el browser (cifrado, accesible via SignalR).

**Decisión:** Almacenar la sesión completa (user + token) en una cookie HttpOnly.

**Razón:**
- HttpOnly cookies son inaccesibles a JavaScript del cliente (protección contra XSS)
- Se transmiten automáticamente en cada request (no requiere SignalR)
- Stateless — cualquier instancia del servidor puede validar la sesión
- SameSite=Lax protege contra CSRF
- El token JWT NUNCA llega al código del cliente

**Consecuencias:** Límite de ~4KB para cookies. Si la sesión crece, considerar almacenamiento server-side con session ID en cookie.

---

### ADR-003: RBAC centralizado en hooks.server.ts

**Contexto:** En Blazor, la protección de rutas se hacía en los layouts con `if (AppService.Autenticado)`, lo cual es cosmético (el servidor no bloquea el acceso).

**Decisión:** Implementar toda la lógica de acceso en `hooks.server.ts`, ejecutada ANTES de resolver la ruta.

**Razón:**
- Seguridad real — el contenido protegido nunca se envía al cliente no autorizado
- Centralizado — un solo lugar para todas las reglas de acceso
- Testeable — las funciones `isPublicRoute()` e `isAuthorized()` son puras y unit-testeable
- Fail-secure — por defecto, requiere autenticación (deny by default)

**Consecuencias:** El UI condicional (ocultar menús por rol) es cosmético y complementario, no la fuente de verdad.

---

### ADR-004: Factory pattern para API client

**Contexto:** .NET usa DI con `AddHttpClient<>` para inyectar HTTP clients configurados.

**Decisión:** Usar factory functions (`createApiClient(token)`) que retornan objetos con métodos HTTP.

**Razón:**
- Cada request obtiene un client con su propio token (scoped al request)
- No hay estado compartido entre requests
- Fácil de testear (pasar un mock client)
- TypeScript provee tipado completo sin necesidad de interfaces formales
- Patrón natural de JavaScript — closures en lugar de clases con DI

**Consecuencias:** No hay interceptors centralizados como en Axios. El manejo de errores debe hacerse en cada función o en un wrapper.

---

### ADR-005: DaisyUI como sistema de componentes

**Contexto:** Blazor usaba Blazored.Toast, BlazorPro.Spinkit, QuickGrid, y CSS custom.

**Decisión:** Usar DaisyUI (plugin de Tailwind CSS) como sistema de componentes base.

**Razón:**
- Componentes semánticos (`btn`, `alert`, `modal`, `table`) sin JavaScript pesado
- Tokens de color semánticos (`primary`, `error`, `success`) para tematización
- Compatible con Tailwind CSS utilities para customización
- No agrega runtime JavaScript — solo clases CSS
- Temas customizables para mantener la identidad visual de SISEC
- Built-in dark mode (futuro)

**Consecuencias:** Algunos componentes complejos (calendar, rich text) necesitarán librerías adicionales.

---

### ADR-006: Tipos TypeScript como contratos de API

**Contexto:** C# usa DTOs tipados para comunicación con la API. TypeScript necesita un equivalente.

**Decisión:** Definir interfaces TypeScript en `$lib/types/` que reflejen los DTOs del backend.

**Razón:**
- Seguridad de tipos en compile-time
- Documentación implícita de la estructura de datos
- Auto-complete en IDE
- Fácil refactoring con TypeScript strict mode
- Las interfaces se eliminan en runtime (zero overhead)

**Consecuencias:** Los tipos deben mantenerse sincronizados manualmente con la API. Idealmente, generar tipos desde OpenAPI spec cuando esté disponible.

---

### ADR-007: Form Actions para mutaciones de datos

**Contexto:** Blazor usa event handlers en C# (`@onclick`, `@onsubmit`) que se ejecutan en el servidor via SignalR.

**Decisión:** Usar SvelteKit Form Actions (`export const actions = { ... }`) para todas las mutaciones.

**Razón:**
- Funciona sin JavaScript (progressive enhancement)
- Server-side validation nativa
- CSRF protection automática
- Flujo claro: form submit → server action → response
- `use:enhance` para UX mejorada sin página completa refresh

**Consecuencias:** Requiere `<form method="POST">` con inputs nombrados. No aplica para interacciones en tiempo real (esas usan fetch API).

---

## 4. Gaps Detectados

| # | Gap | Severidad | Plan de Acción |
|---|-----|-----------|---------------|
| G1 | **Sin documentación de API** — No hay OpenAPI/Swagger spec para la API backend. | 🔴 Crítico | Crear documentación progresiva. Capturar requests del Blazor actual con network inspector. |
| G2 | **Esquema de roles no formalizado** — Los roles se infieren del código, no hay documento oficial. | 🔴 Crítico | Reunión con stakeholders para confirmar roles, permisos, y flujos de acceso. |
| G3 | **Flujo completo de Wompi** — La integración de pagos no está completamente visible en el código frontend. | 🟡 Alto | Revisar documentación de Wompi API. Implementar en sandbox primero. |
| G4 | **PowerBI embed** — La configuración de Azure AD para PowerBI embed no está documentada. | 🟡 Alto | Obtener credenciales de Azure AD del equipo de infraestructura. Probar embed con reporte de test. |
| G5 | **Módulo de enrollment** — El flujo de registro de usuarios es multi-paso y poco documentado. | 🟡 Medio | Mapear flujo completo step-by-step. Posiblemente diferir a una fase posterior. |
| G6 | **reCAPTCHA v3** — El login actual usa reCAPTCHA que requiere configuración adicional. | 🟢 Bajo | Obtener site key y secret key. Implementar verificación server-side. |
| G7 | **Generación de PDF** — PuppeteerSharp sugiere generación de PDFs; el alcance no es claro. | 🟡 Medio | Investigar si se genera en runtime o batch. Evaluar alternativas Node.js. |

---

## 5. Deuda Técnica Planificada

| # | Deuda | Prioridad | Fase |
|---|-------|-----------|------|
| DT1 | **Cifrado de sesión** — Actualmente la cookie de sesión es base64url (decodificable). Debe cifrarse con AES-256. | 🔴 Alta | Pre-producción |
| DT2 | **Tests unitarios** — No hay tests. Necesita Vitest para funciones server, Playwright para E2E. | 🔴 Alta | Fase 10 |
| DT3 | **Módulo de enrollment** — Flujo de registro multi-paso no migrado aún. | 🟡 Media | Fase 8 |
| DT4 | **Integración real con API** — Actualmente los server loads usan datos mock o stubs. | 🔴 Alta | Fase 9 |
| DT5 | **Refresh automático de token** — No implementado; la sesión expira y redirige a login. | 🟡 Media | Fase 9 |
| DT6 | **Manejo de errores global** — `+error.svelte` existe pero no maneja todos los casos edge. | 🟡 Media | Fase 8 |
| DT7 | **Internacionalización** — Todo el UI está en español hardcoded. Si se necesita i18n, requiere refactor. | 🟢 Baja | Futuro |
| DT8 | **Accesibilidad completa** — Checklist parcialmente implementado. Necesita auditoría con axe-core. | 🟡 Media | Fase 10 |
| DT9 | **Performance optimization** — Lazy loading de módulos, code splitting, image optimization. | 🟢 Baja | Post-launch |
| DT10 | **Monitoreo y logging** — Sin integración con Sentry, DataDog, o similar. | 🟡 Media | Pre-producción |

---

## 6. Comparación de Rendimiento: Blazor Server vs SvelteKit

| Métrica | Blazor Server | SvelteKit (adapter-node) |
|---------|:------------:|:------------------------:|
| **Payload inicial** | ~250KB (Blazor runtime) + WebSocket | ~50-80KB (HTML + CSS + minimal JS) |
| **Conexiones persistentes** | 1 WebSocket per user (SignalR) | 0 (HTTP request/response) |
| **Memoria servidor (100 users)** | ~500MB-1GB (circuit state per user) | ~50-100MB (stateless) |
| **Latencia de interacción** | ~50-200ms (round-trip WebSocket) | ~0ms (client-side) / ~50ms (form submit) |
| **Offline capability** | ❌ Ninguna (requiere conexión) | ✅ Parcial (SPA navigation, service worker) |
| **Escalabilidad** | Limitada (afinidad de servidor) | Alta (stateless, horizontal scaling) |
| **SEO** | ⚠️ Limitado (pre-rendering posible) | ✅ SSR nativo |
| **Tiempo hasta interactivo** | ~3-5s (download + WebSocket) | ~1-2s (HTML streamed) |
| **Costo de servidor** | Alto (RAM + CPU por conexión) | Bajo (request-based) |
| **Resiliencia a red** | 🔴 Falla si se pierde WebSocket | 🟢 Degrada gracefully |

### Conclusión de Rendimiento

SvelteKit ofrece ventajas significativas en:
- **Costo operativo** — Sin WebSockets persistentes, el servidor maneja más usuarios con menos recursos
- **Experiencia de usuario** — Menor latencia percibida, funciona sin JavaScript para operaciones básicas
- **Escalabilidad** — Stateless permite horizontal scaling sin afinidad
- **Resiliencia** — No depende de conexión persistente para funcionar

La principal desventaja es la pérdida de actualizaciones en tiempo real (que pueden implementarse con SSE/polling si se necesitan).
