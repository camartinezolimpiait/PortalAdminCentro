# Architecture Notes: SvelteKit BFF Implementation

## BFF (Backend-for-Frontend) Pattern

### Overview
All API communication flows through the SvelteKit server. The browser never makes direct API calls to backend services.

```
Browser ←→ SvelteKit Server ←→ Backend APIs
   │              │                 │
   │   HttpOnly   │    HTTP/HTTPS   │
   │   Cookies    │    with Tokens  │
   │              │                 │
   └──────────────┴─────────────────┘
```

### Server-Side API Clients
Located in `src/lib/server/api-client.ts`:

| Client | Backend Service | Original Named HttpClient |
|---|---|---|
| `sisecAdminClient` | SisecAdmin Parametrization | `HttpClient("SisecAdmin")` |
| `sisecAuthClient` | SisecAuth Authentication | `HttpClient` with uriSisecAuth |
| `strapiClient` | Strapi CMS | `HttpClient("ApiStrapi")` |
| `superTransporteClient` | SuperTransporte SICOV | `HttpClient("ApiSuperT")` |
| `vigiladosClient` | SuperTransporte Vigilados | `HttpClient("ApiSuperVigilados")` |
| `portalAdminClient` | API Gateway Portal Admin | `HttpClient("PortalAdministrativo")` |
| `miLicenciaClient` | MiLicencia Backend | `HttpClient` with ApiFrontMiLicencia |

### Data Loading Pattern
```typescript
// +page.server.ts (server-only)
export const load: PageServerLoad = async ({ locals }) => {
  const apiToken = await getApiToken();
  const data = await portalAdminClient.post('endpoint', body, apiToken.tokenBearer);
  return { data };
};
```

## Session Management

### Flow
1. User submits login form → SvelteKit Form Action
2. Server authenticates against SisecAuth API
3. Server creates session with user info + API token
4. Session encoded as Base64 and stored in HttpOnly cookie
5. Cookie: `sisec_session`, HttpOnly, Secure, SameSite=Lax, 8h max age

### Session Structure
```typescript
interface AuthSession {
  user: SessionUser;  // User profile, roles, menu items
  token: string;      // API gateway token
  expiresAt: number;  // Expiration timestamp
}
```

### Cookie Security Settings
| Setting | Value | Purpose |
|---|---|---|
| `httpOnly` | `true` | Prevent XSS access |
| `secure` | `true` | HTTPS only |
| `sameSite` | `lax` | CSRF protection |
| `maxAge` | 28800 (8h) | Auto-expiration |
| `path` | `/` | Available to all routes |

## hooks.server.ts - Central Auth Guard

### Request Flow
```
1. Request arrives → hooks.server.ts
2. Is route public? → Allow (skip auth)
3. Get session from cookie
4. Session valid? → No → Redirect to /login
5. Session expired? → Clear cookie, redirect to /login
6. Set locals.user and locals.token
7. Check RBAC permissions for route
8. User has required role? → No → Redirect to /403-unauthorized
9. Allow request → resolve(event)
```

### RBAC Resolution
Routes are matched using prefix matching. The most specific match wins.
Example: `/agendamiento/configuracion/cupos` matches `/agendamiento/configuracion` (most specific).

## Layout Hierarchy

```
+layout.svelte (root - imports CSS, renders ToastContainer)
├── login/+page.svelte (standalone, no portal layout)
├── 403-unauthorized/+page.svelte (standalone)
├── +error.svelte (standalone)
└── (authenticated)/+layout.svelte (wraps with PortalLayout)
    ├── pines/* (all pin management pages)
    ├── compra-pin/* (multi-step purchase flow)
    ├── compra-pin-cda/* (CDA purchase flow)
    ├── agendamiento/* (scheduling module)
    ├── facturacion/* (billing module)
    ├── supertransporte/* (regulatory module)
    ├── powerbi/ (reports)
    └── ... (other authenticated pages)
```

## Server Load Functions (+page.server.ts / +layout.server.ts)

### Usage Pattern
| File | Purpose |
|---|---|
| `+layout.server.ts` (root) | Expose `user` to all pages via `$page.data.user` |
| `+page.server.ts` (login) | Load client types, handle login/recovery actions |
| `+page.server.ts` (data pages) | Fetch data from backend APIs |
| `+server.ts` (API routes) | BFF proxy endpoints (catalogs, auth) |

### Form Actions Pattern
```typescript
// +page.server.ts
export const actions: Actions = {
  login: async ({ request, cookies }) => {
    const formData = await request.formData();
    // Validate, authenticate, create session
    // Return fail(400, { error }) or redirect
  }
};
```

## Stores

| Store | Purpose | Replaces |
|---|---|---|
| `toasts` | Notification system | Blazored.Toast |
| `loading` | Global loading indicator | SpinLoader |
| `compraPinStore` | Multi-step purchase state | ApplicationShared + session state |

## Catalog Caching Strategy
- Server-side in-memory cache with 5-minute TTL
- Located at `/api/catalogs` endpoint
- Supports: tiposDocumento, departamentos, municipios, categorias, tramites, sexos, gruposSanguineos, eps
- Deduplicates concurrent requests for the same catalog type

## Environment Configuration
All sensitive configuration is server-side only via `$env/dynamic/private`.
No API keys, tokens, or credentials are exposed to the client.

| Env Variable | Purpose |
|---|---|
| `URI_SISEC_AUTH` | Authentication service URL |
| `URI_SISEC_PARAMETIZATION` | Admin parametrization URL |
| `API_PORTAL_ADMIN_URL` | API Gateway URL |
| `API_PORTAL_ADMIN_USER` | Gateway credentials |
| `API_PORTAL_ADMIN_PASS` | Gateway credentials |
| `RECAPTCHA_SECRET_KEY` | reCAPTCHA server-side key |
| `TOKEN_STRAPI` | Strapi CMS token |
| ... | See `src/lib/server/config.ts` |
