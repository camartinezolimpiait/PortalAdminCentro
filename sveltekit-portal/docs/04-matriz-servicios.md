# Matriz de Servicios C# → TypeScript

## Resumen de Migración

| Concepto Blazor | Equivalente SvelteKit | Ubicación |
|----------------|----------------------|-----------|
| `interface IService` | Módulo TypeScript exportado | `$lib/server/` |
| `AddScoped<IService, Service>()` | `import { fn } from '$lib/server/...'` | `+page.server.ts` |
| `@inject IService svc` | `import` directo en server load | `+page.server.ts` |
| `AddHttpClient<IService>()` | `createApiClient()` factory | `$lib/server/api-client.ts` |
| `ApplicationSevice` (estado global) | `event.locals` + `$page.data` | `hooks.server.ts` → layouts |

---

## 1. ITokenService → `$lib/server/auth.ts`

### Blazor (Antes)

```csharp
// Interface
public interface ITokenService
{
    Task<TokenModel> ObtenerToken(string usuario, string password);
    Task<TokenModel> RefrescarToken(string refreshToken);
}

// Registro DI
services.AddHttpClient<ITokenService, TokenService>(client => {
    client.BaseAddress = new Uri(config["ApiBaseUrl"]);
});

// Uso en Login.razor
@inject ITokenService TokenService
var token = await TokenService.ObtenerToken(usuario, password);
```

### SvelteKit (Después)

```typescript
// $lib/server/auth.ts
import { API_BASE_URL } from '$env/dynamic/private';

export interface TokenResponse {
  tokenBearer: string;
  refreshToken: string;
  expiration: string;
}

export async function obtenerToken(
  usuario: string,
  password: string
): Promise<TokenResponse> {
  const res = await fetch(`${API_BASE_URL}/auth/token`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ usuario, password })
  });
  if (!res.ok) throw new Error('Authentication failed');
  return res.json();
}

export async function refrescarToken(
  refreshToken: string
): Promise<TokenResponse> {
  const res = await fetch(`${API_BASE_URL}/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });
  if (!res.ok) throw new Error('Token refresh failed');
  return res.json();
}
```

```typescript
// src/routes/login/+page.server.ts
import { obtenerToken } from '$lib/server/auth';

export const actions = {
  default: async ({ request, cookies }) => {
    const data = await request.formData();
    const token = await obtenerToken(
      data.get('usuario') as string,
      data.get('password') as string
    );
    cookies.set('session', encodeSession(token), {
      path: '/',
      httpOnly: true,
      secure: true,
      sameSite: 'lax'
    });
    throw redirect(303, '/pines');
  }
};
```

---

## 2. IPerfilService → `$lib/server/auth.ts`

### Blazor (Antes)

```csharp
public interface IPerfilService
{
    Task<PerfilUsuario> ObtenerPerfil(string token);
    Task<List<MenuInfo>> ObtenerMenu(string token, int rolId);
}

services.AddHttpClient<IPerfilService, PerfilService>(client => { ... });
```

### SvelteKit (Después)

```typescript
// $lib/server/auth.ts (se agrega al mismo módulo)
export interface UserProfile {
  nombre: string;
  email: string;
  rolId: number;
  rolNombre: string;
  centroId: number;
  permisos: string[];
}

export async function obtenerPerfil(token: string): Promise<UserProfile> {
  const res = await fetch(`${API_BASE_URL}/perfil`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!res.ok) throw new Error('Profile fetch failed');
  return res.json();
}

export async function obtenerMenu(
  token: string,
  rolId: number
): Promise<MenuItem[]> {
  const res = await fetch(`${API_BASE_URL}/menu/${rolId}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!res.ok) return [];
  return res.json();
}
```

---

## 3. ApplicationSevice → `event.locals` + `$page.data`

### Blazor (Antes)

```csharp
// Clase de estado global inyectada como Scoped
public class ApplicationSevice  // nota: typo en original
{
    public bool Autenticado { get; set; }
    public string TokenBearer { get; set; }
    public string NombreUsuario { get; set; }
    public int RolId { get; set; }
    public string RolNombre { get; set; }
    public int CentroId { get; set; }
    public List<MenuInfo> Menu { get; set; }
}

// Registro
services.AddScoped<ApplicationSevice>();

// Uso en layouts
@inject ApplicationSevice AppService
@if (AppService.Autenticado) { /* mostrar contenido */ }
```

### SvelteKit (Después)

```typescript
// src/app.d.ts
declare global {
  namespace App {
    interface Locals {
      user: {
        nombre: string;
        email: string;
        rolId: number;
        rolNombre: string;
        centroId: number;
        permisos: string[];
      } | null;
      tokenBearer: string | null; // NUNCA llega al cliente
    }
    interface PageData {
      user: {
        nombre: string;
        rolNombre: string;
        centroId: number;
        permisos: string[];
      } | null;
      // tokenBearer NO se incluye aquí
    }
  }
}
```

```typescript
// hooks.server.ts — hidrata event.locals
export const handle: Handle = async ({ event, resolve }) => {
  const session = event.cookies.get('session');
  if (session) {
    const decoded = decodeSession(session);
    event.locals.user = decoded.user;
    event.locals.tokenBearer = decoded.tokenBearer;
  }
  return resolve(event);
};
```

```typescript
// +layout.server.ts — expone datos seguros al cliente
export const load: LayoutServerLoad = async ({ locals }) => {
  return {
    user: locals.user
      ? {
          nombre: locals.user.nombre,
          rolNombre: locals.user.rolNombre,
          centroId: locals.user.centroId,
          permisos: locals.user.permisos
        }
      : null
    // tokenBearer NUNCA se retorna aquí
  };
};
```

---

## 4. IApiService → `$lib/server/api-client.ts`

### Blazor (Antes)

```csharp
public interface IApiService
{
    Task<RespuestaServicios<T>> GetAsync<T>(string endpoint, string token);
    Task<RespuestaServicios<T>> PostAsync<T>(string endpoint, object body, string token);
    Task<RespuestaServicios<T>> PutAsync<T>(string endpoint, object body, string token);
    Task<RespuestaServicios<T>> DeleteAsync<T>(string endpoint, string token);
}
```

### SvelteKit (Después)

```typescript
// $lib/server/api-client.ts
import { API_BASE_URL } from '$env/dynamic/private';
import type { ApiResponse } from '$lib/types/api';

export function createApiClient(token: string) {
  const headers = {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  };

  return {
    async get<T>(endpoint: string): Promise<ApiResponse<T>> {
      const res = await fetch(`${API_BASE_URL}${endpoint}`, { headers });
      return res.json();
    },

    async post<T>(endpoint: string, body: unknown): Promise<ApiResponse<T>> {
      const res = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'POST',
        headers,
        body: JSON.stringify(body)
      });
      return res.json();
    },

    async put<T>(endpoint: string, body: unknown): Promise<ApiResponse<T>> {
      const res = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'PUT',
        headers,
        body: JSON.stringify(body)
      });
      return res.json();
    },

    async del<T>(endpoint: string): Promise<ApiResponse<T>> {
      const res = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'DELETE',
        headers
      });
      return res.json();
    }
  };
}
```

```typescript
// Uso en +page.server.ts
import { createApiClient } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals }) => {
  const api = createApiClient(locals.tokenBearer!);
  const pines = await api.get<Pin[]>('/pines/activos');
  return { pines: pines.data };
};
```

---

## 5. Named HttpClients → Factory Functions

### Blazor (Antes)

```csharp
services.AddHttpClient<ICategoriaService, CategoriaService>(client => {
    client.BaseAddress = new Uri(config["CategoriaApiUrl"]);
    client.DefaultRequestHeaders.Add("X-Api-Key", config["ApiKey"]);
});
```

### SvelteKit (Después)

```typescript
// $lib/server/api-client.ts — factory con configuración específica
import { CATEGORIA_API_URL, API_KEY } from '$env/dynamic/private';

export function createCategoriaClient(token: string) {
  return {
    async getCategorias(): Promise<Categoria[]> {
      const res = await fetch(`${CATEGORIA_API_URL}/categorias`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'X-Api-Key': API_KEY
        }
      });
      return res.json();
    }
  };
}
```

---

## 6–11. Servicios de Agendamiento → `+page.server.ts` loads

| Servicio C# | Módulo SvelteKit | Uso |
|-------------|-----------------|-----|
| `IHorarioAtencionService` | `$lib/server/agendamiento.ts` | `load()` en `/configuracion/horario-atencion/+page.server.ts` |
| `IInicioAgendamiento` | `$lib/server/agendamiento.ts` | `load()` en `/configuracion/horario-agendamiento/+page.server.ts` |
| `IParametrizacionHorarioService` | `$lib/server/agendamiento.ts` | `load()` en `/configuracion/parametrizacion/+page.server.ts` |
| `IConfiguracionCuposReglas` | `$lib/server/agendamiento.ts` | `load()` en `/configuracion/configuracion-cupos/+page.server.ts` |
| `IPerfilAgendaService` | `$lib/server/agendamiento.ts` | `load()` auxiliar |
| `IAgendaService` | `$lib/server/agendamiento.ts` | `load()` en `/agenda/+page.server.ts` |

```typescript
// $lib/server/agendamiento.ts
export async function getHorarioAtencion(api: ReturnType<typeof createApiClient>) {
  return api.get<HorarioAtencion[]>('/agendamiento/horario-atencion');
}

export async function getAgenda(api: ReturnType<typeof createApiClient>, fecha: string) {
  return api.get<Agenda>(`/agendamiento/agenda?fecha=${fecha}`);
}

export async function crearCita(api: ReturnType<typeof createApiClient>, data: NuevaCita) {
  return api.post<Cita>('/agendamiento/citas', data);
}

// ... más funciones exportadas
```

---

## 12. ISuperTransporteService → `$lib/server/supertransporte.ts`

```typescript
// $lib/server/supertransporte.ts
export async function getInfoBasica(api: ReturnType<typeof createApiClient>, centroId: number) {
  return api.get<InfoBasica>(`/supertransporte/centro/${centroId}/info-basica`);
}

export async function postInfoBasica(api: ReturnType<typeof createApiClient>, data: InfoBasica) {
  return api.post<void>('/supertransporte/centro/info-basica', data);
}

// Cada sub-página de SuperTransporte tiene su función correspondiente
export async function getResolucionHabilitacion(api: ReturnType<typeof createApiClient>, centroId: number) { ... }
export async function getPoliza(api: ReturnType<typeof createApiClient>, centroId: number) { ... }
export async function getProfesionalesSalud(api: ReturnType<typeof createApiClient>, centroId: number) { ... }
// ... etc
```

---

## 13. IMiLicenciaService + IApiMilicenciaService → `$lib/server/milicencia.ts`

```typescript
// $lib/server/milicencia.ts
import { MILICENCIA_API_URL } from '$env/dynamic/private';

export async function getPerfil(token: string) {
  const res = await fetch(`${MILICENCIA_API_URL}/perfil`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.json();
}

export async function getDataContact(token: string) {
  const res = await fetch(`${MILICENCIA_API_URL}/contacto`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.json();
}
```

---

## 14. IPermisoService → `$lib/server/rbac.ts`

```typescript
// $lib/server/rbac.ts (ya existe)
export function tienePermiso(user: UserSession, permiso: string): boolean {
  return user.permisos.includes(permiso);
}

export function tieneRol(user: UserSession, ...roles: string[]): boolean {
  return roles.includes(user.rolNombre);
}
```

---

## 15. IPortalAdministrativoService → `$lib/server/portal-admin.ts`

```typescript
// $lib/server/portal-admin.ts
export async function getConfiguracion(api: ReturnType<typeof createApiClient>) {
  return api.get<Configuracion>('/portal/configuracion');
}

export async function updateConfiguracion(api: ReturnType<typeof createApiClient>, data: Configuracion) {
  return api.put<void>('/portal/configuracion', data);
}
```

---

## 16. ICompraPinFlowService + ICompraPinDatosBasicosService → `$lib/server/compra-pin.ts`

```typescript
// $lib/server/compra-pin.ts
export async function getDatosBasicos(api: ReturnType<typeof createApiClient>, tipoDoc: string, documento: string) {
  return api.get<DatosBasicos>(`/compra-pin/datos-basicos?tipoDoc=${tipoDoc}&doc=${documento}`);
}

export async function procesarCompra(api: ReturnType<typeof createApiClient>, data: CompraRequest) {
  return api.post<CompraResponse>('/compra-pin/procesar', data);
}

export async function getMediosPago(api: ReturnType<typeof createApiClient>) {
  return api.get<MedioPago[]>('/compra-pin/medios-pago');
}
```

---

## 17. IReporteService → `$lib/server/reportes.ts`

```typescript
// $lib/server/reportes.ts
import { POWERBI_CLIENT_ID, POWERBI_CLIENT_SECRET, POWERBI_TENANT_ID } from '$env/dynamic/private';

export async function getEmbedToken(reportId: string): Promise<EmbedConfig> {
  // Obtener token de Azure AD via MSAL
  // Obtener embed token de PowerBI API
  // Retornar config para PowerBI JS SDK
}

export async function getReportes(api: ReturnType<typeof createApiClient>): Promise<Reporte[]> {
  return api.get<Reporte[]>('/reportes');
}
```

---

## 18. IOfuscamientoService → `$lib/server/crypto.ts`

```typescript
// $lib/server/crypto.ts
import { SESSION_SECRET } from '$env/dynamic/private';
import crypto from 'node:crypto';

export function encrypt(text: string): string {
  const iv = crypto.randomBytes(16);
  const cipher = crypto.createCipheriv('aes-256-cbc', SESSION_SECRET, iv);
  const encrypted = Buffer.concat([cipher.update(text), cipher.final()]);
  return `${iv.toString('hex')}:${encrypted.toString('hex')}`;
}

export function decrypt(text: string): string {
  const [ivHex, encryptedHex] = text.split(':');
  const decipher = crypto.createDecipheriv(
    'aes-256-cbc',
    SESSION_SECRET,
    Buffer.from(ivHex, 'hex')
  );
  return Buffer.concat([
    decipher.update(Buffer.from(encryptedHex, 'hex')),
    decipher.final()
  ]).toString();
}
```

---

## 19. Servicios Auxiliares

| Servicio C# | Equivalente SvelteKit | Notas |
|-------------|----------------------|-------|
| `DataInformation` | `$lib/server/constants.ts` | Datos estáticos (departamentos, tipos doc, etc.) |
| `IAesEncryptionHelper` | `$lib/server/crypto.ts` | Cifrado AES para datos sensibles |
| `IErrorBoundaryLogger` → `BlazorExceptionLogger` | `+error.svelte` + `handleError` hook | Error boundaries nativos de SvelteKit |
| `ProtectedSessionStorage` | HttpOnly cookies | Eliminación completa del storage en browser |

---

## Diagrama de Flujo: Servicio → Server Load

```
Blazor:
  @inject IService svc  →  svc.Method()  →  HttpClient  →  API

SvelteKit:
  +page.server.ts
    └── import { fn } from '$lib/server/module'
        └── fn(createApiClient(locals.tokenBearer))
            └── fetch(API_BASE_URL + endpoint)  →  API
```
