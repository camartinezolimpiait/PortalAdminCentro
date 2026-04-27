import type { LoginCredentials, LoginResponse, ServerSession } from '$lib/types/auth';
import { createAuthClient, createPortalClient } from './api-client';
import { SESSION_DURATION_MS } from './constants';

export async function authenticate(credentials: LoginCredentials): Promise<LoginResponse> {
  const authClient = createAuthClient();

  try {
    const tokenResponse = await authClient.post<{ tokenBearer: string; message: string }>(
      '/api/Token/ObtenerToken',
      {
        userName: credentials.userName,
        passWord: credentials.password,
        plataforma: credentials.plataforma
      }
    );

    if (!tokenResponse.exito || !tokenResponse.datos?.tokenBearer) {
      return { success: false, message: tokenResponse.mensaje || 'Error de autenticación' };
    }

    const token = tokenResponse.datos.tokenBearer;
    const portalClient = createPortalClient(token);

    const profileResponse = await portalClient.get<{
      userId: string;
      userName: string;
      firstName: string;
      centroUsuario: number;
      codigoRuntCentro: number | null;
      clienteId: number;
      tipoAfisId: number;
      tipoDocumentoId: string;
      idOrigenPin: number;
      roles: string[];
      menuList: unknown[];
      menuListPrincipal: unknown[];
      subMenuList: unknown[];
      listaClientes: unknown[];
      mustChangePassword?: boolean;
      tempPassword?: string;
    }>('/api/Perfil/ObtenerPerfil');

    if (!profileResponse.exito) {
      return { success: false, message: profileResponse.mensaje || 'Error al obtener perfil' };
    }

    const profile = profileResponse.datos;
    return {
      success: true,
      token,
      mustChangePassword: profile.mustChangePassword,
      tempPassword: profile.tempPassword,
      userData: {
        userId: profile.userId,
        userName: profile.userName,
        firstName: profile.firstName,
        centroUsuario: profile.centroUsuario,
        codigoRuntCentro: profile.codigoRuntCentro,
        clienteId: profile.clienteId,
        tipoAfisId: profile.tipoAfisId,
        tipoDocumentoId: profile.tipoDocumentoId,
        idOrigenPin: profile.idOrigenPin,
        roles: (profile.roles || []) as LoginResponse['userData'] extends undefined ? never : NonNullable<LoginResponse['userData']>['roles']
      },
      menuList: profile.menuList as LoginResponse['menuList'],
      menuListPrincipal: profile.menuListPrincipal as LoginResponse['menuListPrincipal'],
      subMenuList: profile.subMenuList as LoginResponse['subMenuList'],
      listaClientes: profile.listaClientes as LoginResponse['listaClientes']
    };
  } catch (error) {
    return { success: false, message: error instanceof Error ? error.message : 'Error de conexión' };
  }
}

export function buildSession(loginResponse: LoginResponse, plataforma: string): ServerSession | null {
  if (!loginResponse.success || !loginResponse.token || !loginResponse.userData) return null;

  const { userData, token, menuList, menuListPrincipal, subMenuList, listaClientes } = loginResponse;

  return {
    userId: userData.userId,
    userName: userData.userName,
    firstName: userData.firstName,
    plataforma: plataforma as ServerSession['plataforma'],
    centroUsuario: userData.centroUsuario,
    codigoRuntCentro: userData.codigoRuntCentro,
    clienteId: userData.clienteId,
    tipoAfisId: userData.tipoAfisId,
    tipoDocumentoId: userData.tipoDocumentoId,
    idOrigenPin: userData.idOrigenPin,
    roles: userData.roles,
    menuList: (menuList ?? []) as ServerSession['menuList'],
    menuListPrincipal: (menuListPrincipal ?? []) as ServerSession['menuListPrincipal'],
    subMenuList: (subMenuList ?? []) as ServerSession['subMenuList'],
    listaClientes: (listaClientes ?? []) as ServerSession['listaClientes'],
    tokenBearer: token,
    expiresAt: Date.now() + SESSION_DURATION_MS
  };
}
