# Conversión de Tipos C# → TypeScript

## Reglas Generales de Conversión

### Tipos Primitivos

| C# | TypeScript | Notas |
|----|-----------|-------|
| `string` | `string` | Idéntico |
| `int` | `number` | JS no distingue int/float |
| `long` | `number` | Cuidado con valores > 2^53 (usar `bigint` si necesario) |
| `float` | `number` | |
| `double` | `number` | |
| `decimal` | `number` | Perder precisión; considerar `string` para montos |
| `bool` | `boolean` | |
| `DateTime` | `string` | ISO 8601 format (`"2024-01-15T10:30:00Z"`) |
| `DateTimeOffset` | `string` | ISO 8601 con offset |
| `TimeSpan` | `string` | Formato `"HH:mm:ss"` o número de segundos |
| `Guid` | `string` | UUID como string |
| `byte[]` | `string` | Base64-encoded |
| `char` | `string` | |

### Tipos Nullable

| C# | TypeScript | Notas |
|----|-----------|-------|
| `string?` | `string \| null` | En JSON: `null` o ausente |
| `int?` | `number \| null` | |
| `bool?` | `boolean \| null` | |
| `DateTime?` | `string \| null` | |

### Colecciones

| C# | TypeScript | Notas |
|----|-----------|-------|
| `List<T>` | `T[]` | Array literal |
| `IEnumerable<T>` | `T[]` | Se serializa como array |
| `ICollection<T>` | `T[]` | |
| `Dictionary<string, T>` | `Record<string, T>` | |
| `Dictionary<int, T>` | `Record<number, T>` | Keys se convierten a string en JSON |
| `T[]` | `T[]` | Idéntico |
| `IReadOnlyList<T>` | `readonly T[]` | |

### Tipos Especiales

| C# | TypeScript | Notas |
|----|-----------|-------|
| `object` | `unknown` | Evitar `any` |
| `dynamic` | `unknown` | |
| `Task<T>` | `Promise<T>` | |
| `void` | `void` | |
| `Tuple<T1, T2>` | `[T1, T2]` | |
| `enum` | `enum` o `string union` | Preferir string unions |

---

## Convenciones de Nomenclatura

| Convención C# | Convención TypeScript | Ejemplo |
|--------------|----------------------|---------|
| PascalCase (propiedades) | camelCase (propiedades) | `NombreUsuario` → `nombreUsuario` |
| PascalCase (clases) | PascalCase (interfaces/types) | `TokenModel` → `TokenResponse` |
| `I` prefix (interfaces) | Sin prefix | `ITokenService` → `TokenService` (type) |
| PascalCase (métodos) | camelCase (funciones) | `ObtenerToken()` → `obtenerToken()` |
| `_` prefix (privados) | Sin prefix (o `#` private) | `_token` → `token` (private) |
| UPPER_CASE (constantes) | UPPER_CASE | `MAX_RETRIES` → `MAX_RETRIES` |
| `Dto` suffix | Sin suffix | `InfoBasicaDto` → `InfoBasica` |
| `Model` suffix | Sin suffix | `TokenModel` → `TokenResponse` |

---

## Conversiones Específicas del Proyecto

### 1. ApplicationSevice → UserSession

```csharp
// C# (Antes)
public class ApplicationSevice
{
    public bool Autenticado { get; set; }
    public string TokenBearer { get; set; }
    public string NombreUsuario { get; set; }
    public int RolId { get; set; }
    public string RolNombre { get; set; }
    public int CentroId { get; set; }
    public List<MenuInfo> Menu { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface UserSession {
  nombre: string;
  email: string;
  rolId: number;
  rolNombre: string;
  centroId: number;
  permisos: string[];
}

// Nota: `Autenticado` ya no es necesario (user != null implica autenticado)
// Nota: `TokenBearer` se almacena en event.locals, NO en UserSession
// Nota: `Menu` se deriva de `permisos` + `rolNombre` en el cliente
```

### 2. MenuInfo → MenuItem

```csharp
// C# (Antes)
public class MenuInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Icono { get; set; }
    public string Url { get; set; }
    public int? PadreId { get; set; }
    public List<MenuInfo> SubMenus { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface MenuItem {
  id: number;
  nombre: string;
  icono: string;
  url: string;
  padreId: number | null;
  subMenus: MenuItem[];
}
```

### 3. RespuestaServicios → ApiResponse

```csharp
// C# (Antes)
public class RespuestaServicios<T>
{
    public bool Estado { get; set; }
    public string Mensaje { get; set; }
    public T Data { get; set; }
    public List<string> Errores { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface ApiResponse<T> {
  estado: boolean;
  mensaje: string;
  data: T;
  errores: string[];
}
```

### 4. TokenModel → TokenResponse

```csharp
// C# (Antes)
public class TokenModel
{
    public string TokenBearer { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface TokenResponse {
  tokenBearer: string;
  refreshToken: string;
  expiration: string; // ISO 8601
}
```

### 5. DTOs de SuperTransporte

```csharp
// C# (Antes)
public class InfoBasicaDto
{
    public int Id { get; set; }
    public string NombreCentro { get; set; }
    public string Nit { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public int DepartamentoId { get; set; }
    public int MunicipioId { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface InfoBasica {
  id: number;
  nombreCentro: string;
  nit: string;
  direccion: string;
  telefono: string;
  email: string;
  departamentoId: number;
  municipioId: number;
  fechaCreacion: string; // ISO 8601
}
```

### 6. Modelos de CompraPin

```csharp
// C# (Antes)
public class DatosBasicosModel
{
    public string TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; }
    public string PrimerNombre { get; set; }
    public string SegundoNombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface DatosBasicos {
  tipoDocumento: string;
  numeroDocumento: string;
  primerNombre: string;
  segundoNombre: string;
  primerApellido: string;
  segundoApellido: string;
  fechaNacimiento: string; // ISO 8601
  genero: string;
}
```

### 7. Modelos de Facturación

```csharp
// C# (Antes)
public class ConfigurarFacturacionModel
{
    public string RazonSocial { get; set; }
    public string Nit { get; set; }
    public bool FacturacionElectronica { get; set; }
    public string Prefijo { get; set; }
    public int ResolucionNumero { get; set; }
    public DateTime ResolucionFecha { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface ConfiguracionFacturacion {
  razonSocial: string;
  nit: string;
  facturacionElectronica: boolean;
  prefijo: string;
  resolucionNumero: number;
  resolucionFecha: string; // ISO 8601
}
```

### 8. Modelos de Agendamiento

```csharp
// C# (Antes)
public class HorarioAtencionModel
{
    public int DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public bool Activo { get; set; }
    public int CuposMaximos { get; set; }
}
```

```typescript
// TypeScript (Después)
export interface HorarioAtencion {
  diaSemana: number; // 0=Dom, 1=Lun, ..., 6=Sab
  horaInicio: string; // "08:00"
  horaFin: string;    // "17:00"
  activo: boolean;
  cuposMaximos: number;
}
```

---

## Enums: C# → TypeScript

### Preferencia: String Unions

```csharp
// C# (Antes)
public enum EstadoPin
{
    Activo = 1,
    Usado = 2,
    Vencido = 3,
    Devuelto = 4
}
```

```typescript
// TypeScript (Después) — String Union (preferido)
export type EstadoPin = 'activo' | 'usado' | 'vencido' | 'devuelto';

// O si se necesitan valores numéricos:
export enum EstadoPin {
  Activo = 1,
  Usado = 2,
  Vencido = 3,
  Devuelto = 4
}
```

### Mapeo de Enums Conocidos

| Enum C# | TypeScript | Valores |
|---------|-----------|---------|
| `EstadoPin` | `type EstadoPin` | `'activo' \| 'usado' \| 'vencido' \| 'devuelto'` |
| `TipoDocumento` | `type TipoDocumento` | `'CC' \| 'CE' \| 'TI' \| 'PA' \| 'NIT'` |
| `TipoTramite` | `type TipoTramite` | `'primera-vez' \| 'renovacion' \| 'recategorizacion'` |
| `Genero` | `type Genero` | `'M' \| 'F' \| 'O'` |
| `DiaSemana` | `number` | `0-6` (estándar JS) |
| `MedioPago` | `type MedioPago` | `'pse' \| 'tarjeta' \| 'efectivo' \| 'wompi'` |

---

## Patrones de Conversión

### Genéricos

```csharp
// C# genérico
public class RespuestaServicios<T> { ... }
public class PolizaDtoRequest<T, X> { ... }
```

```typescript
// TypeScript genérico
export interface ApiResponse<T> { ... }
export interface PolizaRequest<T, X> { ... }
```

### Herencia → Composición

```csharp
// C# con herencia
public class DatosPersonalesCDA : DatosPersonalesBase
{
    public string TipoVehiculo { get; set; }
}
```

```typescript
// TypeScript con intersección
interface DatosPersonalesBase {
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
}

export interface DatosPersonalesCDA extends DatosPersonalesBase {
  tipoVehiculo: string;
}

// O con intersección:
export type DatosPersonalesCDA = DatosPersonalesBase & {
  tipoVehiculo: string;
};
```

### Clases de Servicio → Funciones Puras

```csharp
// C# clase de servicio
public class CategoriaService : ICategoriaService
{
    private readonly HttpClient _httpClient;
    
    public CategoriaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Categoria>> ObtenerCategorias()
    {
        var response = await _httpClient.GetAsync("/categorias");
        return await response.Content.ReadAsAsync<List<Categoria>>();
    }
}
```

```typescript
// TypeScript función pura
export async function obtenerCategorias(
  api: ReturnType<typeof createApiClient>
): Promise<Categoria[]> {
  const response = await api.get<Categoria[]>('/categorias');
  return response.data;
}
```

---

## Checklist de Conversión

- [ ] Renombrar propiedades de PascalCase a camelCase
- [ ] Eliminar sufijos `Dto`, `Model` innecesarios
- [ ] Convertir `DateTime` a `string` (ISO 8601)
- [ ] Convertir `TimeSpan` a `string` (formato `HH:mm`)
- [ ] Convertir `decimal` a `number` (o `string` para montos exactos)
- [ ] Convertir `Guid` a `string`
- [ ] Convertir enums a string unions donde sea posible
- [ ] Eliminar prefijo `I` de interfaces
- [ ] Convertir `List<T>` a `T[]`
- [ ] Convertir nullable types a `T | null`
- [ ] Verificar que genéricos se mapeen correctamente
- [ ] Convertir herencia a extends o intersección de tipos
