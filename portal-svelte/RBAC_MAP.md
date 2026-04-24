# RBAC Map: Role-Based Access Control

## Implementation
RBAC is enforced in `src/hooks.server.ts` as a central server-side guard.
Every request is intercepted before page resolution.

## Supported Roles
| Role | Description |
|---|---|
| `CRC` | Centro de Reconocimiento de Conductores |
| `CEA` | Centro de Enseñanza Automovilística |
| `CDA` | Centro de Diagnóstico Automotriz |
| `Director` | Administrative director role |
| `Auditor` | Audit and compliance role |
| `Investigador` | Investigation role |
| `Instructor` | Teaching/instruction role |

## Route → Role Mapping

### Public Routes (No Authentication Required)
| Route | Description |
|---|---|
| `/login` | Login page |
| `/api/auth/login` | Login API endpoint |
| `/api/auth/logout` | Logout API endpoint |
| `/api/auth/recover-password` | Password recovery |
| `/403-unauthorized` | Access denied page |

### Protected Routes

| Route | Allowed Roles | Original Blazor Equivalent |
|---|---|---|
| `/pines` | CRC, CEA, CDA, Director, Auditor, Investigador | `@layout SingleLayout` |
| `/pines/activos` | CRC, CEA, CDA, Director, Auditor, Investigador | `@layout Pines` |
| `/pines/usados` | CRC, CEA, CDA, Director, Auditor, Investigador | `@layout Pines` |
| `/pines/devoluciones` | CRC, CEA, CDA, Director, Auditor | `@layout Pines` |
| `/pines/dispersiones` | CRC, CEA, CDA, Director, Auditor | `@layout Pines` |
| `/pines/certificado-ingreso` | CRC, CEA, CDA, Director | `@layout Pines` |
| `/pines/busqueda` | CRC, CEA, CDA, Director, Auditor, Investigador | — |
| `/pines/asociados` | CRC, CEA, CDA, Director, Auditor, Investigador | — |
| `/compra-pin` | CRC, CEA, Director, Auditor | `@layout TwoPaneLayout` |
| `/compra-pin-cda` | CDA, Director | — |
| `/agendamiento` | CRC, CEA, CDA, Director, Instructor | `@layout TwoPaneLayout` |
| `/agendamiento/agenda` | CRC, CEA, CDA, Director, Instructor | `@layout PortalLayout` |
| `/agendamiento/configuracion` | CRC, CEA, CDA, Director | `@layout Configuracion` |
| `/facturacion` | CRC, CEA, CDA, Director, Auditor | — |
| `/facturacion/configurar` | CRC, CEA, CDA, Director | `@layout Configuracion` |
| `/facturacion/consultar` | CRC, CEA, CDA, Director, Auditor | `@layout SingleLayout` |
| `/supertransporte` | CRC, CEA, Director, Auditor, Investigador | `@layout SupertransporteLayout` |
| `/supertransporte/centro` | CRC, Director, Auditor, Investigador | `@layout SupertransporteLayout` |
| `/supertransporte/centro-cea` | CEA, Director, Auditor, Investigador | `@layout SupertransporteLayout` |
| `/supertransporte/crc` | CRC, Director, Auditor, Investigador | `@layout FormularioLayout` |
| `/supertransporte/cea` | CEA, Director, Auditor, Investigador | `@layout FormularioLayout` |
| `/supertransporte/pqrsf` | CRC, CEA, Director, Auditor | `@layout SingleLayout` |
| `/powerbi` | CRC, CEA, CDA, Director, Auditor | `@layout PowerBILayout` |
| `/crear-usuario` | Director | — |
| `/cambiar-contrasena` | All authenticated | — |
| `/gestionar-pin` | CRC, CEA, CDA, Director | — |
| `/valida-pin-crc` | CRC, Director | `@layout SingleLayout` |
| `/valida-pin-cea` | CEA, Director | `@layout SingleLayout` |
| `/pago-cuota` | CRC, CEA, CDA, Director | — |
| `/devolucion` | CRC, CEA, CDA, Director, Auditor | — |

## Redirections
| Condition | Redirect |
|---|---|
| No session + protected route | → `/login` |
| Session + insufficient role | → `/403-unauthorized` |
| No session + API call | → 401 JSON response |
| Session + insufficient role + API | → 403 JSON response |
| Authenticated user visits `/` | → `/pines` (or first menu item) |

## Blazor Attribute Equivalence
| Blazor Pattern | SvelteKit Equivalent |
|---|---|
| `[Authorize]` | Route not in `PUBLIC_ROUTES` set |
| `[Authorize(Roles = "Director")]` | `ROLE_ROUTES['/route'] = ['Director']` |
| `AuthorizeView` | `{#if data.user?.roles.includes('Director')}` |
| `@attribute [Authorize]` on page | Central `hooks.server.ts` guard |
