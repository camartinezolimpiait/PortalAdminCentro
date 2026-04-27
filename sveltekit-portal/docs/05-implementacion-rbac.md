# Implementación RBAC en hooks.server.ts

## Arquitectura General

```
                  ┌──────────────────────────────────────────────────┐
                  │                  REQUEST FLOW                     │
                  └──────────────────────────────────────────────────┘

  Browser Request
       │
       ▼
  ┌─────────────┐    ¿Cookie "session"     ┌──────────────────┐
  │ hooks.server │───── existe? ──────────►│ decodeSession()  │
  │    .ts       │       SÍ                │ base64url decode │
  │              │                         │ JSON.parse       │
  │              │                         │ check expiry     │
  │              │◄────────────────────────│ return user      │
  │              │                         └──────────────────┘
  │              │       NO
  │              │────────┐
  │              │        │
  │  Populate    │        │
  │  event.locals│        │
  │  .user       │        ▼
  │  .tokenBearer│   user = null
  │              │   tokenBearer = null
  └──────┬───────┘
         │
         ▼
  ┌─────────────┐     ¿Ruta pública?      ┌──────────────────┐
  │ Route Check  │──── SÍ ───────────────►│ resolve(event)   │
  │              │                         │ (continuar)      │
  │              │                         └──────────────────┘
  │              │       NO
  │              │
  │              │     ¿user == null?
  │              │──── SÍ ───────────────► redirect(303, '/login')
  │              │
  │              │       NO
  │              │
  │              │     ¿Rol autorizado?
  │              │──── NO ───────────────► redirect(303, '/403-unauthorized')
  │              │
  │              │       SÍ
  │              │─────────────────────────► resolve(event)
  └──────────────┘
```

---

## Hidratación de `event.locals.user`

### Lectura de Cookie

```typescript
// hooks.server.ts
import type { Handle } from '@sveltejs/kit';
import { redirect } from '@sveltejs/kit';
import { decodeSession } from '$lib/server/session';
import { isPublicRoute, isAuthorized } from '$lib/server/rbac';

export const handle: Handle = async ({ event, resolve }) => {
  // 1. Leer cookie HttpOnly
  const sessionCookie = event.cookies.get('session');

  if (sessionCookie) {
    try {
      // 2. Decodificar y validar
      const session = decodeSession(sessionCookie);
      event.locals.user = session.user;
      event.locals.tokenBearer = session.tokenBearer;
    } catch (e) {
      // Cookie inválida o expirada → limpiar
      event.cookies.delete('session', { path: '/' });
      event.locals.user = null;
      event.locals.tokenBearer = null;
    }
  } else {
    event.locals.user = null;
    event.locals.tokenBearer = null;
  }

  // 3. Verificar acceso a ruta
  const pathname = event.url.pathname;

  if (!isPublicRoute(pathname) && !event.locals.user) {
    throw redirect(303, '/login');
  }

  if (event.locals.user && !isAuthorized(pathname, event.locals.user)) {
    throw redirect(303, '/403-unauthorized');
  }

  return resolve(event);
};
```

### Decodificación de Sesión

```typescript
// $lib/server/session.ts
import type { UserSession } from '$lib/types/auth';

interface SessionPayload {
  user: UserSession;
  tokenBearer: string;
  exp: number; // Unix timestamp
}

export function encodeSession(payload: SessionPayload): string {
  const json = JSON.stringify(payload);
  return Buffer.from(json).toString('base64url');
}

export function decodeSession(cookie: string): SessionPayload {
  const json = Buffer.from(cookie, 'base64url').toString('utf-8');
  const payload: SessionPayload = JSON.parse(json);

  // Verificar expiración
  if (Date.now() > payload.exp * 1000) {
    throw new Error('Session expired');
  }

  return payload;
}
```

---

## Rutas Públicas vs Protegidas

### Rutas Públicas (no requieren autenticación)

```typescript
// $lib/server/rbac.ts
const PUBLIC_ROUTES = [
  '/login',
  '/enrollment',
  '/enrollment/create-user',
  '/enrollment/residence',
  '/enrollment/additional',
  '/enrollment/supplementary-2',
  '/enrollment/supplementary-3',
  '/enrollment/photo',
  '/enrollment/auth-data',
  '/enrollment/barcode',
  '/enrollment/child',
  '/reject',
  '/success',
  '/fingerprints',
  '/api/auth/logout'
];

const PUBLIC_PREFIXES = [
  '/enrollment/',
  '/api/auth/'
];

export function isPublicRoute(pathname: string): boolean {
  if (PUBLIC_ROUTES.includes(pathname)) return true;
  return PUBLIC_PREFIXES.some(prefix => pathname.startsWith(prefix));
}
```

### Rutas Protegidas con Roles

```typescript
// $lib/server/rbac.ts
interface RouteRule {
  pattern: string | RegExp;
  roles: string[];
}

const ROUTE_RULES: RouteRule[] = [
  // Pines — Admin, Auditor
  { pattern: '/pines', roles: ['Admin', 'Auditor', 'Director'] },

  // CompraPin — Admin
  { pattern: /^\/compradepin/, roles: ['Admin'] },
  { pattern: /^\/comprapincda/, roles: ['Admin'] },

  // Configuración — Admin, Director
  { pattern: /^\/configuracion/, roles: ['Admin', 'Director'] },

  // Facturación consulta — Admin, Auditor
  { pattern: '/consultar-facturacion', roles: ['Admin', 'Auditor'] },

  // Agenda — Admin, Instructor
  { pattern: '/agenda', roles: ['Admin', 'Instructor'] },

  // SuperTransporte — Admin, Director
  { pattern: /^\/supertransporte/, roles: ['Admin', 'Director'] },

  // Reportes — Admin, Auditor
  { pattern: '/reportes', roles: ['Admin', 'Auditor'] },

  // Validación de PINes — Admin
  { pattern: '/validar-pin-crc', roles: ['Admin'] },
  { pattern: '/validar-pin-cea', roles: ['Admin'] },

  // Cambio de contraseña — Todos los autenticados
  { pattern: '/change-password', roles: ['Admin', 'Auditor', 'Director', 'Instructor', 'Investigador'] },
];

export function isAuthorized(pathname: string, user: UserSession): boolean {
  const rule = ROUTE_RULES.find(r =>
    typeof r.pattern === 'string'
      ? pathname === r.pattern || pathname.startsWith(r.pattern + '/')
      : r.pattern.test(pathname)
  );

  // Si no hay regla específica, solo requiere estar autenticado
  if (!rule) return true;

  return rule.roles.includes(user.rolNombre);
}
```

---

## Tabla de Acceso por Rol

| Módulo | Admin | Auditor | Director | Instructor | Investigador |
|--------|:-----:|:-------:|:--------:|:----------:|:------------:|
| `/pines` | ✅ | ✅ | ✅ | ❌ | ❌ |
| `/compradepin/*` | ✅ | ❌ | ❌ | ❌ | ❌ |
| `/comprapincda/*` | ✅ | ❌ | ❌ | ❌ | ❌ |
| `/configuracion/*` | ✅ | ❌ | ✅ | ❌ | ❌ |
| `/consultar-facturacion` | ✅ | ✅ | ❌ | ❌ | ❌ |
| `/agenda` | ✅ | ❌ | ❌ | ✅ | ❌ |
| `/supertransporte/*` | ✅ | ❌ | ✅ | ❌ | ❌ |
| `/reportes` | ✅ | ✅ | ❌ | ❌ | ❌ |
| `/validar-pin-*` | ✅ | ❌ | ❌ | ❌ | ❌ |
| `/change-password` | ✅ | ✅ | ✅ | ✅ | ✅ |

---

## Datos en `$page.data.user` vs Datos Server-Only

### Lo que SÍ llega al cliente (`$page.data.user`)

```typescript
{
  nombre: "Juan Pérez",
  rolNombre: "Admin",
  centroId: 42,
  permisos: ["pines.ver", "pines.crear", "config.editar"]
}
```

**Propósito:** Renderizado condicional en UI (mostrar/ocultar elementos según rol).

### Lo que NUNCA llega al cliente

```typescript
// event.locals (solo accesible en server)
{
  tokenBearer: "eyJhbGciOiJIUzI1NiIs...",  // Token JWT del API
  user: {
    email: "juan@centro.com",               // PII sensible
    // ... datos completos
  }
}
```

**Principio:** Need-to-know. El cliente solo recibe lo mínimo para renderizar UI.

---

## Renderizado Condicional por Rol en UI

```svelte
<!-- +layout.svelte o cualquier componente -->
<script>
  import { page } from '$app/stores';

  $: user = $page.data.user;
  $: isAdmin = user?.rolNombre === 'Admin';
  $: isAuditor = user?.rolNombre === 'Auditor';
  $: canManagePines = isAdmin || isAuditor;
</script>

<!-- Menú condicional -->
<nav>
  {#if canManagePines}
    <a href="/pines">Gestión de PINes</a>
  {/if}

  {#if isAdmin}
    <a href="/compradepin">Compra de PIN</a>
    <a href="/configuracion">Configuración</a>
  {/if}

  {#if user}
    <a href="/change-password">Cambiar Contraseña</a>
  {/if}
</nav>
```

> **Nota:** La UI condicional es cosmética. La **verdadera protección** está en `hooks.server.ts`. Incluso si se manipula el DOM, las rutas protegidas redirigen en el servidor.

---

## Flujo Completo de Autenticación

```
1. Usuario envía formulario de login
   POST /login (Form Action)

2. +page.server.ts llama obtenerToken() → API externa
   Recibe: { tokenBearer, refreshToken, expiration }

3. +page.server.ts llama obtenerPerfil(token) → API externa
   Recibe: { nombre, email, rolId, rolNombre, permisos }

4. Construye SessionPayload y lo codifica en base64url
   cookies.set('session', encoded, { httpOnly: true, secure: true })

5. Redirect a /pines (o dashboard)

6. Siguiente request → hooks.server.ts:
   - Lee cookie 'session'
   - Decodifica base64url → JSON
   - Verifica expiración
   - Popula event.locals.user y event.locals.tokenBearer
   - Verifica acceso a ruta
   - Resuelve si autorizado

7. +layout.server.ts expone datos seguros a $page.data.user
   (sin tokenBearer)
```

---

## Diferencias con el Sistema Original Blazor

| Aspecto | Blazor Server | SvelteKit |
|---------|--------------|-----------|
| Protección de rutas | Layout-level (cosmética) | hooks.server.ts (server-side, real) |
| Almacenamiento de token | ProtectedSessionStorage (browser) | HttpOnly cookie (inaccesible a JS) |
| Estado de sesión | ApplicationSevice (in-memory per circuit) | Cookie stateless per-request |
| Validación de acceso | `if (AppService.Autenticado)` en UI | Middleware en cada request |
| Exposición de token | Accesible via SignalR | Nunca llega al cliente |
| CSRF | SignalR (WebSocket, no aplica) | SameSite=Lax + Form Actions |
