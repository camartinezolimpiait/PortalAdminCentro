# Reemplazo de Inyección de Dependencias .NET → SvelteKit

## Tabla Resumen de Conceptos

| Concepto .NET DI | Equivalente SvelteKit | Ejemplo |
|-----------------|----------------------|---------|
| Service Registration (`AddScoped`) | Module exports | `export function fn() {}` |
| `@inject IService` | `import { fn } from '$lib/server/...'` | Import directo |
| Scoped lifetime | Per-request `load()` functions | Cada load es un request nuevo |
| Singleton lifetime | Module-level instances | `const client = new Client()` |
| Transient lifetime | Factory functions | `createApiClient(token)` |
| `IOptions<T>` / `IConfiguration` | `$env/dynamic/private` | `import { API_URL } from '$env/dynamic/private'` |
| `NavigationManager` | `goto()` / `redirect()` | `throw redirect(303, '/login')` |
| `ProtectedSessionStorage` | HttpOnly cookies | `event.cookies.set('session', ...)` |

---

## 1. Service Registration → Module Exports

### .NET (Antes)

```csharp
// Startup.cs / StartupExtensions.cs
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IPerfilService, PerfilService>();
services.AddScoped<IApiService, ApiService>();
services.AddScoped<ApplicationSevice>();
```

### SvelteKit (Después)

```typescript
// No hay "registro". Los módulos exportan funciones directamente.
// La resolución de dependencias es un simple import.

// $lib/server/auth.ts
export async function obtenerToken(usuario: string, password: string) { ... }
export async function obtenerPerfil(token: string) { ... }

// $lib/server/api-client.ts
export function createApiClient(token: string) { ... }

// Uso en +page.server.ts:
import { obtenerToken } from '$lib/server/auth';
import { createApiClient } from '$lib/server/api-client';
```

**¿Por qué funciona?** En .NET, DI resuelve grafos de dependencias en runtime. En SvelteKit, los módulos de ES son el grafo de dependencias — TypeScript verifica en compile-time que todas las dependencias existen.

---

## 2. @inject → import

### .NET (Antes)

```csharp
// En un componente .razor
@inject ITokenService TokenService
@inject IPerfilService PerfilService
@inject ApplicationSevice AppService
@inject NavigationManager NavManager

// En code-behind .razor.cs
[Inject] private IApiService ApiService { get; set; }
```

### SvelteKit (Después)

```typescript
// En +page.server.ts (server-side)
import { obtenerToken } from '$lib/server/auth';
import { createApiClient } from '$lib/server/api-client';
import { redirect } from '@sveltejs/kit';

export const load: PageServerLoad = async ({ locals }) => {
  const api = createApiClient(locals.tokenBearer!);
  // usar api...
};
```

```svelte
<!-- En +page.svelte (client-side) -->
<script>
  import { goto } from '$app/navigation';
  import { page } from '$app/stores';
  import AlertMessage from '$lib/components/ui/AlertMessage.svelte';

  // Datos del server load
  export let data;
</script>
```

---

## 3. Scoped → Per-Request Load Functions

### .NET (Antes)

```csharp
// AddScoped: una instancia por request/circuit de SignalR
services.AddScoped<IAgendaService, AgendaService>();

// El servicio mantiene estado durante el lifecycle del circuito
public class AgendaService : IAgendaService
{
    private readonly HttpClient _httpClient;
    private List<Cita> _citasCache;

    public AgendaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Cita>> ObtenerCitas(DateTime fecha)
    {
        _citasCache = await _httpClient.GetFromJsonAsync<List<Cita>>(...);
        return _citasCache;
    }
}
```

### SvelteKit (Después)

```typescript
// Cada load function es inherentemente "scoped" al request
// No hay estado compartido entre requests

// src/routes/agenda/+page.server.ts
import { createApiClient } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals, url }) => {
  // Se crea un nuevo api client por request (scoped)
  const api = createApiClient(locals.tokenBearer!);

  const fecha = url.searchParams.get('fecha') || new Date().toISOString();
  const citas = await api.get<Cita[]>(`/agendamiento/citas?fecha=${fecha}`);

  return { citas: citas.data };
  // El api client se descarta al finalizar el request
};
```

**Equivalencia:** `AddScoped` = cada `load()` function crea sus propias instancias. No hay estado compartido entre requests.

---

## 4. Singleton → Module-Level Instances

### .NET (Antes)

```csharp
// Una sola instancia para toda la vida de la aplicación
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<ICacheService, MemoryCacheService>();
```

### SvelteKit (Después)

```typescript
// $lib/server/cache.ts
// Las variables a nivel de módulo persisten entre requests
// (equivalente a Singleton en la misma instancia del proceso)

const cache = new Map<string, { data: unknown; expires: number }>();

export function getCached<T>(key: string): T | null {
  const entry = cache.get(key);
  if (!entry || Date.now() > entry.expires) {
    cache.delete(key);
    return null;
  }
  return entry.data as T;
}

export function setCache(key: string, data: unknown, ttlMs: number): void {
  cache.set(key, { data, expires: Date.now() + ttlMs });
}
```

```typescript
// $lib/server/constants.ts
// Constantes son inherentemente singleton
export const DEPARTAMENTOS = [
  { id: 1, nombre: 'Amazonas' },
  { id: 2, nombre: 'Antioquia' },
  // ...
];
```

**Nota:** En producción con múltiples instancias (cluster mode), el "singleton" es por proceso. Para estado compartido entre procesos, usar Redis u otro almacenamiento externo.

---

## 5. Transient → Factory Functions

### .NET (Antes)

```csharp
// Nueva instancia cada vez que se solicita
services.AddTransient<IEmailSender, EmailSender>();
services.AddTransient<IPdfGenerator, PdfGenerator>();
```

### SvelteKit (Después)

```typescript
// $lib/server/api-client.ts
// Factory function: crea una nueva instancia cada vez que se llama

export function createApiClient(token: string) {
  // Cada llamada crea un nuevo objeto con su propio token
  return {
    async get<T>(endpoint: string): Promise<ApiResponse<T>> {
      const res = await fetch(`${API_BASE_URL}${endpoint}`, {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        }
      });
      return res.json();
    },
    // ... post, put, del
  };
}

// Uso: cada load crea su propia instancia
export const load = async ({ locals }) => {
  const api = createApiClient(locals.tokenBearer!); // transient
  // ...
};
```

---

## 6. IOptions<T> / IConfiguration → `$env/dynamic/private`

### .NET (Antes)

```csharp
// appsettings.json
{
  "ApiSettings": {
    "BaseUrl": "https://api.sisec.gov.co",
    "ApiKey": "secret-key-123",
    "Timeout": 30
  },
  "PowerBI": {
    "TenantId": "...",
    "ClientId": "...",
    "ClientSecret": "..."
  }
}

// Registro
services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

// Uso
public class ApiService
{
    private readonly ApiSettings _settings;
    
    public ApiService(IOptions<ApiSettings> options)
    {
        _settings = options.Value;
    }
}
```

### SvelteKit (Después)

```bash
# .env (NO commitear)
API_BASE_URL=https://api.sisec.gov.co
API_KEY=secret-key-123
API_TIMEOUT=30
POWERBI_TENANT_ID=...
POWERBI_CLIENT_ID=...
POWERBI_CLIENT_SECRET=...
SESSION_SECRET=random-32-char-string
```

```typescript
// $lib/server/api-client.ts
import {
  API_BASE_URL,
  API_KEY,
  API_TIMEOUT
} from '$env/dynamic/private';

export function createApiClient(token: string) {
  const timeout = parseInt(API_TIMEOUT || '30') * 1000;
  // ...
}
```

```typescript
// $lib/server/reportes.ts
import {
  POWERBI_TENANT_ID,
  POWERBI_CLIENT_ID,
  POWERBI_CLIENT_SECRET
} from '$env/dynamic/private';
```

**Ventajas:**
- Variables de entorno son el estándar de la industria (12-factor app)
- `$env/dynamic/private` NUNCA expone valores al cliente
- Tipado automático por SvelteKit
- Fácil override en Docker/K8s

---

## 7. NavigationManager → `goto()` / `redirect()`

### .NET (Antes)

```csharp
@inject NavigationManager NavManager

// Navegación programática
NavManager.NavigateTo("/pines");
NavManager.NavigateTo("/login", forceLoad: true);
NavManager.NavigateTo("/configuracion", replace: true);

// Obtener URL actual
var uri = NavManager.Uri;
var baseUri = NavManager.BaseUri;
```

### SvelteKit (Después)

```typescript
// En server-side (+page.server.ts, hooks.server.ts)
import { redirect } from '@sveltejs/kit';

// Redirect (solo en server)
throw redirect(303, '/pines');
throw redirect(303, '/login');
```

```svelte
<!-- En client-side (+page.svelte) -->
<script>
  import { goto } from '$app/navigation';
  import { page } from '$app/stores';

  // Navegación programática
  async function irAPines() {
    await goto('/pines');
  }

  // Con replace (no agrega al historial)
  async function irALogin() {
    await goto('/login', { replaceState: true });
  }

  // URL actual
  $: currentPath = $page.url.pathname;
  $: searchParams = $page.url.searchParams;
</script>

<!-- Navegación declarativa (preferida) -->
<a href="/pines">Ir a PINes</a>
```

---

## 8. ProtectedSessionStorage → HttpOnly Cookies

### .NET (Antes)

```csharp
@inject ProtectedSessionStorage SessionStorage

// Guardar
await SessionStorage.SetAsync("token", tokenBearer);
await SessionStorage.SetAsync("perfil", perfilUsuario);

// Leer
var result = await SessionStorage.GetAsync<string>("token");
if (result.Success)
{
    var token = result.Value;
}

// Eliminar
await SessionStorage.DeleteAsync("token");
```

### SvelteKit (Después)

```typescript
// Guardar sesión (en +page.server.ts o hooks)
event.cookies.set('session', encodeSession({
  user: { nombre, email, rolId, rolNombre, centroId, permisos },
  tokenBearer,
  exp: Math.floor(Date.now() / 1000) + 3600 // 1 hora
}), {
  path: '/',
  httpOnly: true,    // Inaccesible a JavaScript del cliente
  secure: true,      // Solo HTTPS
  sameSite: 'lax',   // Protección CSRF
  maxAge: 3600       // 1 hora
});

// Leer sesión (en hooks.server.ts)
const sessionCookie = event.cookies.get('session');
if (sessionCookie) {
  const { user, tokenBearer } = decodeSession(sessionCookie);
  event.locals.user = user;
  event.locals.tokenBearer = tokenBearer;
}

// Eliminar sesión
event.cookies.delete('session', { path: '/' });
```

**Diferencias clave:**

| Aspecto | ProtectedSessionStorage | HttpOnly Cookies |
|---------|------------------------|-----------------|
| Almacenamiento | Browser (sessionStorage cifrado) | Browser (cookie HttpOnly) |
| Acceso JS | Cifrado pero en el DOM | Inaccesible a JS |
| Persistencia | Por tab del browser | Cross-tab |
| Transmisión | Via SignalR (round-trip) | Automática en cada HTTP request |
| Seguridad | Depende de cifrado del servidor | Inaccesible al cliente |
| Escalabilidad | Requiere afinidad de servidor | Stateless (cualquier servidor) |

---

## Diagrama de Equivalencias

```
┌──────────────────────────────────────────────────┐
│                  .NET DI Container                │
│                                                  │
│  services.AddScoped<ISvc, Svc>()                 │
│       │                                          │
│       ▼                                          │
│  Container resuelve ISvc                         │
│       │                                          │
│       ▼                                          │
│  @inject ISvc svc                                │
│       │                                          │
│       ▼                                          │
│  svc.Method()                                    │
└──────────────────────────────────────────────────┘

                    ═══ SE CONVIERTE EN ═══

┌──────────────────────────────────────────────────┐
│              ES Modules + Imports                 │
│                                                  │
│  // $lib/server/module.ts                        │
│  export function method() { ... }                │
│       │                                          │
│       ▼                                          │
│  // +page.server.ts                              │
│  import { method } from '$lib/server/module';    │
│       │                                          │
│       ▼                                          │
│  const result = await method();                  │
└──────────────────────────────────────────────────┘
```

---

## Checklist de Migración DI

- [x] Identificar todos los `AddScoped`, `AddSingleton`, `AddTransient`
- [x] Crear módulos TypeScript equivalentes en `$lib/server/`
- [x] Reemplazar `@inject` con `import`
- [x] Convertir constructores con DI a funciones con parámetros explícitos
- [x] Mover configuración de `appsettings.json` a `.env`
- [x] Reemplazar `IOptions<T>` con `$env/dynamic/private`
- [x] Reemplazar `NavigationManager` con `goto`/`redirect`
- [x] Reemplazar `ProtectedSessionStorage` con HttpOnly cookies
- [ ] Reemplazar `IErrorBoundaryLogger` con `handleError` hook
- [ ] Migrar `AddHttpClient<>` a factory functions con fetch
