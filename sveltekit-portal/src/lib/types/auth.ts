/** User roles in SISEC system */
export type UserRole = 'Auditor' | 'Investigador' | 'Director' | 'Instructor';

/** Platform types */
export type Plataforma = 'CRC' | 'CEA' | 'CDA' | 'Armas';

/** Menu item - mapped from MenuInfo.cs */
export interface MenuItem {
  id: number;
  padre: number;
  aplicacionId: number;
  esOpcionMenu: string;
  ruta: string;
  orden: number;
  activo: string;
  pagina: string;
  userId: string;
  pageName: string;
  menuName: string;
  iconoNombre: string;
}

/** Client/Platform entity - mapped from ClienteDTO.cs */
export interface ClienteDTO {
  id: number;
  nombre: string;
  applicationId: string;
}

/** User session stored in HttpOnly cookie (client-safe) */
export interface UserSession {
  userId: string;
  userName: string;
  firstName: string;
  plataforma: Plataforma;
  centroUsuario: number;
  codigoRuntCentro: number | null;
  clienteId: number;
  tipoAfisId: number;
  tipoDocumentoId: string;
  idOrigenPin: number;
  roles: UserRole[];
  menuList: MenuItem[];
  menuListPrincipal: MenuItem[];
  subMenuList: MenuItem[];
  listaClientes: ClienteDTO[];
  /** Token NEVER reaches client */
  tokenBearer?: never;
}

/** Server-only session (includes token) */
export interface ServerSession extends Omit<UserSession, 'tokenBearer'> {
  tokenBearer: string;
  expiresAt: number;
}

/** Login credentials - mapped from UserData.cs */
export interface LoginCredentials {
  userName: string;
  password: string;
  plataforma: string;
  recaptchaToken?: string;
}

/** Login API response */
export interface LoginResponse {
  success: boolean;
  token?: string;
  message?: string;
  mustChangePassword?: boolean;
  tempPassword?: string;
  userData?: {
    userId: string;
    userName: string;
    firstName: string;
    centroUsuario: number;
    codigoRuntCentro: number | null;
    clienteId: number;
    tipoAfisId: number;
    tipoDocumentoId: string;
    idOrigenPin: number;
    roles: UserRole[];
  };
  menuList?: MenuItem[];
  menuListPrincipal?: MenuItem[];
  subMenuList?: MenuItem[];
  listaClientes?: ClienteDTO[];
}
